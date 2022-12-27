using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SpotListAPI.Models;
using SpotListAPI.Services;

namespace SpotListAPI.Services
{
    public class PlaylistService
    {
        private readonly ILogger<PlaylistService> _logger;
        private readonly UserService _userService;
        private readonly TrackService _trackService;
        private readonly SpotifyService _spotifyService;
        private readonly AudioFeaturesService _audioFeaturesService;
        private readonly Helper _helper;
        private readonly IMemoryCache _cache;

        public PlaylistService (ILogger<PlaylistService> logger,
                                IMemoryCache cache,
                                UserService userService ,   
                                TrackService trackService,
                                SpotifyService spotifyService,
                                AudioFeaturesService audioFeaturesService,
                                Helper helper)
        {
            _logger = logger;
            _cache = cache;
            _userService = userService;
            _trackService = trackService;
            _spotifyService = spotifyService;
            _audioFeaturesService = audioFeaturesService;
            _helper = helper;
        }

        public async Task<PlaylistResponse> CopyPlaylist(PlaylistRequest playlistRequest)
        {
            playlistRequest.UserId = await _userService.GetUser(playlistRequest.Auth);

            var p = await GetPlaylists(playlistRequest);
            var name = p.Where(a => a.Id == playlistRequest.Id).First().Title;

                playlistRequest.Name = "Copy of " + name;
       

            //clear the cache
            _cache.Remove(playlistRequest.UserId + "/tracks");
            _cache.Remove(playlistRequest.UserId + "/playlists");
            //create the playlist
            playlistRequest.OldPlaylistId = playlistRequest.Id;
            playlistRequest.Id = await AddBlankPlaylist(playlistRequest);


            //add tracks to the playlist
            var addTracksResponse = await _trackService.AddTracksToPlaylist(playlistRequest, true);
            addTracksResponse.Title = playlistRequest.Name;
            return addTracksResponse;
        }


        //create/list playlist url users/{user_id}/playlists
        public async Task<PlaylistResponse> CreatePlaylist(PlaylistRequest playlistRequest)
        {
            playlistRequest.UserId = await _userService.GetUser(playlistRequest.Auth);
            
            //Get audio features from specific playlist and use that information
            if (playlistRequest.AudioFeatures)
            {
                var tracks = await _trackService.GetTracksFromPlaylist(
                    new GetPlaylistTracksRequest() { Id = playlistRequest.Id,
                                                    Auth = playlistRequest.Auth,
                                                    UserId = playlistRequest.UserId });
                var audioFeatures = await _audioFeaturesService.GetAudioDataFromTracks(tracks, playlistRequest.Auth);

                playlistRequest.Dance = audioFeatures.Dance;
                playlistRequest.Energy = audioFeatures.Energy;
                playlistRequest.Tempo = audioFeatures.Tempo;
                playlistRequest.Valence = audioFeatures.Valence;
                playlistRequest.StandardDeviation = audioFeatures.StandardDeviation;
                playlistRequest.Genres = audioFeatures.Genres;

            }

            if (playlistRequest.SavedTracks)
            {
                playlistRequest.Name = playlistRequest.UserId + " Saved Tracks";
            }

            //clear the cache
            _cache.Remove(playlistRequest.UserId + "/tracks");
            _cache.Remove(playlistRequest.UserId + "/playlists");
            //create the playlist
            playlistRequest.Id = await AddBlankPlaylist(playlistRequest);


            //add tracks to the playlist
            var addTracksResponse= await _trackService.AddTracksToPlaylist(playlistRequest);
            addTracksResponse.Title = playlistRequest.Name;
            return addTracksResponse;
        }
        
        //Add blank playlist to be filled
        public async Task<string> AddBlankPlaylist(PlaylistRequest playlistRequest)
        {
            var paramDict = new Dictionary<string, string>();
            paramDict.Add("name", playlistRequest.Name);
            var url = string.Format("users/{0}/playlists", playlistRequest.UserId);
            var jsonParams = JsonSerializer.Serialize(paramDict);
            var addPlaylistResponse = await _spotifyService.SpotifyApi(playlistRequest.Auth, url, "post", jsonParams);            
            //return the id
            var playlist = _helper.Mapper<Playlist>(await addPlaylistResponse.Content.ReadAsByteArrayAsync());
            
            return playlist.Id;
        }

        //Get user's playlists
        //TODO cache this
        public async Task<List<PlaylistResponse>> GetPlaylist(PlaylistRequest playlistRequest)
        {
            var playlistList = new List<Playlist>();

            var playlists = new List<Playlist>();
            var url = string.Format($"playlists/{playlistRequest.Id}");
              
            var getPlaylistsResponse = await _spotifyService.SpotifyApi(playlistRequest.Auth, url, "get");

            var getPlaylist = _helper.Mapper<Playlist>(await getPlaylistsResponse.Content.ReadAsByteArrayAsync());

            playlists.Add(getPlaylist);
               
            playlistList = playlists;
            

            return PlaylistToPlaylistResponse(playlistList);
        }
        
        //Get user's playlists
        //TODO cache this
        public async Task<List<PlaylistResponse>> GetPlaylists(PlaylistRequest playlistRequest)
        {
            var playlistList = new List<Playlist>();
            var user = await _userService.GetUser(playlistRequest.Auth);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            //_cache.Remove(user + "/playlists");

            if (!_cache.TryGetValue(user+"/playlists", out playlistList))
            {
                var playlists = new List<Playlist>();
                var url = string.Format("me/playlists");
                var getPlaylists = new PaginatedPlaylistResponse()
                {
                    next = ""
                };
                //deal with pagination
                while (getPlaylists.next !=null)
                {
                    var getPlaylistsResponse = await _spotifyService.SpotifyApi(playlistRequest.Auth, url, "get");

                    getPlaylists = _helper.Mapper<PaginatedPlaylistResponse>(await getPlaylistsResponse.Content.ReadAsByteArrayAsync());

                    playlists.AddRange(getPlaylists.items);

                    url = string.Format("me/playlists?offset={0}",getPlaylists.limit+getPlaylists.offset); 

                }
                //Populate tracks in playlist to get length
                if (!playlistRequest.UpdateTracks)
                {
                    foreach (var playlist in playlists)
                    {
                        var t = await _trackService.GetTracksFromPlaylist(new GetPlaylistTracksRequest()
                        {
                            Auth = playlistRequest.Auth,
                            Id = playlist.Id,
                            UserId = playlist.Owner.Id
                        });
                        if (t != null)
                        {
                            playlist.Tracks.items = t.ToArray();
                        }

                    }

                }
                playlistList = playlists;
                _cache.Set(user + "/playlists", playlists);
            }

            return PlaylistToPlaylistResponse(playlistList);
        }

        //updates a given playlist with your saved tracks
        public async Task<PlaylistResponse> UpdatePlaylist(PlaylistRequest playlistRequest)
        {
            playlistRequest.UserId = await _userService.GetUser(playlistRequest.Auth);            

            //clear the cache
            _cache.Remove(playlistRequest.UserId + "/tracks");
            _cache.Remove(playlistRequest.UserId + "/playlists");

            //get the tracks
            var savedTracks = await _trackService.GetSavedTracks(playlistRequest);

            var existingTracks = await _trackService.GetTracksFromPlaylist(new GetPlaylistTracksRequest() { Id = playlistRequest.Id, Auth = playlistRequest.Auth, UserId = playlistRequest.UserId });
            //get the difference in tracks?
            var existingUris = existingTracks.Select(e => e.Uri);
            var newTracks = savedTracks.Where(s => !existingUris.Contains(s));
            playlistRequest.TrackIds = newTracks.ToArray();
            //add tracks to the playlist
            var addTracksResponse = await _trackService.AddTracksToPlaylist(playlistRequest);
            addTracksResponse.Title = playlistRequest.Name;
            return addTracksResponse;
        }

        //"Deletes" a playlist actually unfollows it
        internal async Task<string> UnfollowPlaylist(PlaylistRequest playlistRequest)
        {
            var user = await _userService.GetUser(playlistRequest.Auth);
            var url = string.Format("playlists/{0}/followers", playlistRequest.Id);
            var unfollowResponse = await _spotifyService.SpotifyApi(playlistRequest.Auth, url, "delete");
            _cache.Remove(user+"/playlists");
            return unfollowResponse.StatusCode.ToString();
        }
        //Mapper to return specific data
        public List<PlaylistResponse> PlaylistToPlaylistResponse(List<Playlist> playlists)
        {
            var playlistResponse = new List<PlaylistResponse>();
            foreach (var p in playlists)
            {
                var len = p.Tracks.items !=null ? p.Tracks.items.Sum(x => x.DurationMs) : 0;
                var cnt = p.Tracks.items != null ? p.Tracks.items.Length : 0;
                playlistResponse.Add(new PlaylistResponse
                {
                    Id=p.Id,
                    Title = p.Name,
                    Length = len,
                    TrackCount = cnt
                });
            }
            return playlistResponse;
        }
    }
}
