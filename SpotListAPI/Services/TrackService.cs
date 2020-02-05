using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using SpotListAPI.Models;

namespace SpotListAPI.Services
{
    public class TrackService
    {
        private readonly IMemoryCache _cache;
        private readonly SpotifyService _spotifyService;
        private readonly UserService _userService;
        private readonly Helper _helper;
        public TrackService(IMemoryCache cache, SpotifyService spotifyService, UserService userService, Helper helper)
        {
            _cache = cache;
            _spotifyService = spotifyService;
            _userService = userService;
            _helper = helper;
        }

        //add tracks to playlist
        public async Task<PlaylistResponse> AddTracksToPlaylist(PlaylistRequest playlistRequest)
        {
            var paramDict = new Dictionary<string, string[]>();
            var url = string.Format("playlists/{0}/tracks",playlistRequest.Id);
            //get the recomendations
            var tracks = await GetRecommendedTracks(playlistRequest);
            //add the tracks
            var trackLength = 0;
            var trackList = new List<string>();
            foreach (var t in tracks)
            {
                if (trackLength <= playlistRequest.Length * 60000)
                {
                    trackLength += t.DurationMs;
                    trackList.Add(t.Uri);
                }
                else
                {
                    break;
                }
            }
    
            paramDict.Add("uris", trackList.ToArray());

            var trackStringJson =JsonSerializer.Serialize(paramDict);

            var addTracksResponse = await _spotifyService.SpotifyApi(playlistRequest.Auth, url, "post", trackStringJson);          

            return new PlaylistResponse() { Id = playlistRequest.Id, Length = trackLength, TrackCount = trackList.Count }; 
        }
        //Gets the tracks from a given playlist
        //TODO Cache this
        public async Task<List<Track>> GetTracksFromPlaylist(GetPlaylistTracksRequest playlistTracksRequest)
        {
            var user = await _userService.GetUser(playlistTracksRequest.Auth);
            var playlistTracks = new List<Track>();
            if (!_cache.TryGetValue(user+"/"+ playlistTracksRequest.Id+"/tracks", out playlistTracks))
            {
                var playlistTracksList = new List<Track>();
                var url = string.Format("playlists/{0}/tracks", playlistTracksRequest.Id);
                var tracks = new PaginatedPlaylistTrackResponse()
                {
                    next = ""
                };
                while (tracks.next !=null)
                {
                    //get tracks from spotify
                    var trackResponse = await _spotifyService.SpotifyApi(playlistTracksRequest.Auth, url, "get");
                    Track[] trackArray;
                    //parse the tracks
                    tracks = _helper.Mapper<PaginatedPlaylistTrackResponse>(await trackResponse.Content.ReadAsByteArrayAsync());
                    if (tracks.items.Length>0)
                    {
                        trackArray = tracks.items.Select(t => t.Track).ToArray();
                        playlistTracksList.AddRange(trackArray);
                    }

                    if (tracks.limit.HasValue && tracks.offset.HasValue)
                    {
                        url = string.Format("playlists/{0}/tracks?offset={1}", playlistTracksRequest.Id, tracks.limit + tracks.offset);
                    }
                    
                }
                playlistTracks = playlistTracksList;
                //do some magic to get the proper response
                _cache.Set(user + "/" + playlistTracksRequest.Id + "/tracks", playlistTracksList);
            }
            

            return playlistTracks;
        }
        //Uses specific parameters to return recommended songs
        public async Task<List<Track>> GetRecommendedTracks (PlaylistRequest playlistRequest)
        {
            //get a ballpark limit could make this more precise
            var limitRec = playlistRequest.Length / 3;
            var url = string.Format("recommendations?limit={0}&market=from_token&",limitRec.ToString());
            if (playlistRequest.Artist != null && playlistRequest.Artist.Trim() !="")
            {
                playlistRequest.Artist = await SearchArtistFromName(playlistRequest);
            }
            var parsedParams = GetParsedParams(playlistRequest);

            var getRecommendedTracksResponse = await _spotifyService.SpotifyApi(playlistRequest.Auth, url+parsedParams, "get");

            var getRecommendedTracks = _helper.Mapper<RecommendedTracksResponse>(await getRecommendedTracksResponse.Content.ReadAsByteArrayAsync()).tracks;

            playlistRequest.Length = getRecommendedTracks.Length;

            return getRecommendedTracks.ToList();
        }

        public async Task<Dictionary<string,PlaylistRequest>> GetAudioDataFromTracks(List<Track> tracks, string auth)
        {
            //TODO This can totally be its own helper function "Get X of something"

            var numTracks = tracks.Count;
            int trackChunks = (numTracks + 99) / 100;
            //break into 100 song chunks
            var trackArray = new List<string>[trackChunks];

            for (int i = 0; i < trackChunks; i++)
            {
                var intTracks = tracks.Take(100).Skip(i * 100).ToList();
                trackArray[i] = intTracks.Select(i => i.Id).ToList();
            }


            //get audio features from spotify track ids (max 100 per request)
            var afUrl = "https://api.spotify.com/v1/audio-features?ids=";
            var listAudioFeatures = new List<AudioFeatures>();
            foreach (var t in trackArray)
            {
                var ids = string.Join(",", t);
                var getAudioFeaturesResponse = await _spotifyService.SpotifyApi(auth, afUrl+ids, "get");
                var getAudioFeatures = _helper.Mapper<AudioFeaturesResponse>(await getAudioFeaturesResponse.Content.ReadAsByteArrayAsync());
                listAudioFeatures.AddRange(getAudioFeatures.AudioFeatures);
            }

            //get genres from the artists
            var artistList = new List<Artist>();

            foreach (var t in tracks)
            {
                artistList.AddRange(t.Artists);
            }

            var artistIds = artistList.Select(a => a.Id).ToList();

            var numArtists = artistIds.Count;
            int artistChunks = (numArtists + 49) / 50;
            //break into 100 song chunks
            var artistArray = new List<string>[artistChunks];

            for (int i = 0; i < artistChunks; i++)
            {
                artistArray[i] = artistIds.Take(50).Skip(i * 50).ToList();
            }

            //Get Spotify artist ids from tracks (max 50 per request)
            var artistUrl = "https://api.spotify.com/v1/artists?ids=";
            var fullArtistList = new List<FullArtist>();

            foreach (var a in artistArray)
            {
                var ids = string.Join(",", a);
                var getArtistResponse = await _spotifyService.SpotifyApi(auth, artistUrl + ids, "get");
                var getArtists = _helper.Mapper<FullArtistsResponse>(await getArtistResponse.Content.ReadAsByteArrayAsync());
                fullArtistList.AddRange(getArtists.Artists);
            }

            var genresList = new List<string>();

            foreach (var a in fullArtistList)
            {
                genresList.AddRange(a.Genres);
            }

            //need to have a trackaudiofeatures model

            //do some math use std deviation do min/max within 2 std deviation
            //"minAudioFeatures" and "maxAudioFeatures" dict
            //tempo dance energy valence 

            return new Dictionary<string, PlaylistRequest>();
        }

        #region Helper Functions
        //Gets artist id from name for use in recommendation
        private async Task<string> SearchArtistFromName(PlaylistRequest playlistRequest)
        {
            var artistString = "";
            var artistSplit = playlistRequest.Artist.Split(',');
            foreach (var artist in artistSplit)
            {
                if (artist != "")
                {
                    var artistEncoded = artist.Replace(" ", "%20");
                    var url = string.Format("search?q={0}&type=artist&limit=1", artistEncoded);

                    var artistSearchResponse = await _spotifyService.SpotifyApi(playlistRequest.Auth, url, "get");

                    var artistSearch = _helper.Mapper<ArtistsResponse>(await artistSearchResponse.Content.ReadAsByteArrayAsync());
                    if (artistSearch.artists.items.Length > 0)
                    {
                        artistString += "," + artistSearch.artists.items[0].Id;
                    }
                }
            }
            return artistString.Substring(1,artistString.Length-1);
        }
        //gets parameters into a query string
        public string GetParsedParams(PlaylistRequest p)
        {
            var paramString = "";
            if (p.Genres.Length > 0) { paramString += "seed_genres=" + string.Join(",", p.Genres).Trim(); };
            if (p.Artist != "" && p.Artist!=null) { paramString += "&seed_artists=" + p.Artist; };
            if (p.Tempo != null) { paramString += "&target_tempo=" + p.Tempo; }
            if (p.Dance != null) { paramString += "&target_danceability=" + p.Dance; }
            if (p.Energy != null) { paramString += "&target_energy=" + p.Energy; }
            if (p.Valence != null) { paramString += "&target_valence=" + p.Valence; }
            return paramString;
        }

        //Mapper to return back only info needed
        public List<TrackResponse> TracksToTrackResponse (List<Track> tracks)
        {
            var trackResponse = new List<TrackResponse>();
            foreach (var t in tracks)
            {
                trackResponse.Add(new TrackResponse
                {
                    Title = t.Name,
                    Length = t.DurationMs,
                    Artists = t.Artists.FirstOrDefault().Name
                });
            }
            return trackResponse;
        }

        #endregion
    }
}
