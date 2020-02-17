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
            //Refactor to allow 100 request max mainly for saved tracks playlist creation
            var paramDict = new Dictionary<string, string[]>();
            var tracks = new List<Track>();
            var trackList = new List<string>();
            var trackLength = 0;
            var url = string.Format("playlists/{0}/tracks",playlistRequest.Id);

            //if not getting the saved tracks get recommendations
            if (!playlistRequest.SavedTracks)
            {
                //get the recomendations
                tracks = await GetRecommendedTracks(playlistRequest);
                //add the tracks           
                
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
            }
            else
            {
                trackList = await GetSavedTracks(playlistRequest);
            }

            var chunkTracks = _helper.ChunkBy<string>(trackList, 100);
            foreach (var chunk in chunkTracks)
            {
                paramDict.Clear();
                paramDict.Add("uris", trackList.ToArray());

                var trackStringJson =JsonSerializer.Serialize(paramDict);

                var addTracksResponse = await _spotifyService.SpotifyApi(playlistRequest.Auth, url, "post", trackStringJson);   
            }

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
            //hard stop at 100 tracks TODO paginate this request at a later date
            if (limitRec>100)
            {
                limitRec = 100;
            }

            var url = string.Format("recommendations?limit={0}&market=from_token&",limitRec.ToString());

            if (playlistRequest.Artist != null && playlistRequest.Artist.Trim() !="")
            {
                playlistRequest.Artist = await SearchArtistFromName(playlistRequest);
            }

            var parsedParams = !playlistRequest.AudioFeatures ? GetParsedParams(playlistRequest) : GetAudioParsedParams(playlistRequest);

            var getRecommendedTracksResponse = await _spotifyService.SpotifyApi(playlistRequest.Auth, url+parsedParams, "get");

            var getRecommendedTracks = _helper.Mapper<RecommendedTracksResponse>(await getRecommendedTracksResponse.Content.ReadAsByteArrayAsync()).tracks;

            //Not sure why this was here to begin with
            //playlistRequest.Length = getRecommendedTracks.Length;

            return getRecommendedTracks.ToList();
        }

        public async Task<List<string>> GetSavedTracks(PlaylistRequest playlistRequest)
        {
            var user = await _userService.GetUser(playlistRequest.Auth);

            var url = "me/tracks";
            var savedTracksList = new List<SavedTrack>();
            var tracks = new PaginatedSavedTrackResponse()
            {
                next = ""
            };
            while (tracks.next != null)
            {
                //get tracks from spotify
                var trackResponse = await _spotifyService.SpotifyApi(playlistRequest.Auth, url, "get");
                SavedTrack[] trackArray;
                //parse the tracks

                //BUG This isn't deserializing properly
                var why = await trackResponse.Content.ReadAsStringAsync();
                tracks = _helper.Mapper<PaginatedSavedTrackResponse>(await trackResponse.Content.ReadAsByteArrayAsync());
                if (tracks.items.Length > 0)
                {
                    trackArray = tracks.items.Select(t => t.Track).ToArray();
                    savedTracksList.AddRange(trackArray);
                }

                if (tracks.limit.HasValue && tracks.offset.HasValue)
                {
                    url = string.Format("me/tracks?offset={1}", playlistRequest.Id, tracks.limit + tracks.offset);
                }

            }
            _cache.Set(user + "/" + playlistRequest.Id + "/tracks",playlistRequest);

            var savedUris = savedTracksList.Select(s => s.Id).ToList();


            return savedUris;
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

        private string GetAudioParsedParams(PlaylistRequest p)
        {
            var paramString = "";
            if (p.Genres.Length > 0) { paramString += "seed_genres=" + string.Join(",", p.Genres).Trim()+"&"; };
            if (p.Tempo != null)
            {
                var tempo = float.Parse(p.Tempo);
                paramString += "min_tempo=" + (tempo-p.StandardDeviation.Tempo).ToString();
                paramString += "&max_tempo=" + (tempo+p.StandardDeviation.Tempo).ToString();
            }
            if (p.Dance != null)
            {
                var dance = float.Parse(p.Dance);
                paramString += "&min_danceability=" + (dance - p.StandardDeviation.Danceability).ToString();
                paramString += "&max_danceability=" + (dance + p.StandardDeviation.Danceability).ToString();
            }
            if (p.Energy != null)
            {
                var energy = float.Parse(p.Energy);
                paramString += "&min_energy=" + (energy - p.StandardDeviation.Energy).ToString();
                paramString += "&max_energy=" + (energy + p.StandardDeviation.Energy).ToString();
            }
            if (p.Valence != null)
            {
                var valence = float.Parse(p.Valence);
                paramString += "&min_valence=" + (valence - p.StandardDeviation.Valence).ToString();
                paramString += "&max_valence=" + (valence + p.StandardDeviation.Valence).ToString();
            }
            if (paramString.Last()=='&')
            {
                paramString = paramString.Substring(0, paramString.Length - 1);
            }
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
