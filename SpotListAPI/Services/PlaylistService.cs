using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
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
        private readonly Helper _helper;

        public PlaylistService (ILogger<PlaylistService> logger,
                                UserService userService ,   
                                TrackService trackService,
                                SpotifyService spotifyService,
                                Helper helper)
        {
            _logger = logger;
            _userService = userService;
            _trackService = trackService;
            _spotifyService = spotifyService;
            _helper = helper;
        }

        //create/list playlist url users/{user_id}/playlists
        public async Task<PlaylistResponse> CreatePlaylist(PlaylistRequest playlistRequest)
        {
            playlistRequest.UserId = await _userService.GetUser(playlistRequest.Auth);
            //create the playlist
            playlistRequest.Id = await AddBlankPlaylist(playlistRequest);
            //add tracks to the playlist
            var addTracksResponse= await _trackService.AddTracksToPlaylist(playlistRequest);
            addTracksResponse.Title = playlistRequest.Name;
            return addTracksResponse;
        }
        //update playlist details url playlists/{playlist_id}

        public async Task<string> AddBlankPlaylist(PlaylistRequest playlistRequest)
        {
            var url = string.Format("users/{0}/playlists", playlistRequest.UserId);
            var jsonParams = JsonSerializer.Serialize(new KeyValuePair<string,string>("name",playlistRequest.Name));
            var addPlaylistResponse = await _spotifyService.SpotifyApi(playlistRequest.Auth, url, "post", jsonParams);            
            //return the id
            var playlist = _helper.Mapper<Playlist>(await addPlaylistResponse.Content.ReadAsByteArrayAsync());
            
            return playlist.Id;
        }

        public async Task<List<PlaylistResponse>> GetPlaylists (PlaylistRequest playlistRequest)
        {
            var url = string.Format("me/playlists");

            var getPlaylistsResponse = await _spotifyService.SpotifyApi(playlistRequest.Auth, url, "get");

            var getPlaylists = _helper.Mapper<List<Playlist>>(await getPlaylistsResponse.Content.ReadAsByteArrayAsync());

            return PlaylistToPlaylistResponse(getPlaylists);
        }

        public List<PlaylistResponse> PlaylistToPlaylistResponse(List<Playlist> playlists)
        {
            var playlistResponse = new List<PlaylistResponse>();
            foreach (var p in playlists)
            {
                playlistResponse.Add(new PlaylistResponse
                {
                    Id=p.Id,
                    Title = p.Name,
                    Length = p.Tracks.Sum(x=>x.DurationMs),
                    TrackCount = p.Tracks.Length
                });
            }
            return playlistResponse;
        }
    }
}
