using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using SpotListAPI.Models;

namespace SpotListAPI.Services
{
    public class TrackService
    {
        private readonly SpotifyService _spotifyService;
        private readonly Helper _helper;
        public TrackService(SpotifyService spotifyService, Helper helper)
        {
            _spotifyService = spotifyService;
            _helper = helper;
        }
        //add/get tracks url playlists/{playlist_id}/tracks
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

            var addTracks = addTracksResponse.StatusCode.ToString();

            return new PlaylistResponse() { Id = playlistRequest.Id, Length = trackLength, TrackCount = tracks.Count }; 
        }

        public async Task<List<TrackResponse>> GetTracksFromPlaylist(GetPlaylistTracksRequest playlistTracksRequest)
        {
            var url = string.Format("playlists/{0}/tracks", playlistTracksRequest.Id);
            //get tracks from spotify
            var trackResponse = await _spotifyService.SpotifyApi(playlistTracksRequest.Auth, url, "get");

            //parse the tracks
            var tracks = _helper.Mapper<List<Track>>(await trackResponse.Content.ReadAsByteArrayAsync());

            //do some magic to get the proper response

            return TracksToTrackResponse(tracks);
        }

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

            return getRecommendedTracks.ToList();
        }

        private async Task<string> SearchArtistFromName(PlaylistRequest playlistRequest)
        {
            var artistEncoded = playlistRequest.Artist.Replace(" ", "%20");
            var url = string.Format("search?q={0}&type=artist",artistEncoded);

            var artistSearchResponse = await _spotifyService.SpotifyApi(playlistRequest.Auth, url, "get");

            var artistSearch = _helper.Mapper<ArtistsResponse>(await artistSearchResponse.Content.ReadAsByteArrayAsync());

            return artistSearch.artists.items[0].Id;
        }
        #region Helper Functions
        public string GetParsedParams(PlaylistRequest p)
        {
            var paramString = "";
            if (p.Genres.Length > 0) { paramString += "seed_genres=" + string.Join(",", p.Genres).Trim(); };
            if (p.Artist != "" && p.Artist!=null) { paramString += "&seed_artists=" + p.Artist; };
            paramString += "&target_tempo=" + p.Tempo.ToString();
            paramString += "&target_danceability=" +p.Dance.ToString();
            paramString += "&target_energy=" + p.Energy.ToString();
            paramString += "&target_instrumentalness=" + p.Instrumental.ToString();
            return paramString;
        }

        public List<TrackResponse> TracksToTrackResponse (List<Track> tracks)
        {
            var trackResponse = new List<TrackResponse>();
            foreach (var t in tracks)
            {
                trackResponse.Add(new TrackResponse
                {
                    Title = t.Name,
                    Length = t.DurationMs,
                    Artists = t.Artists.Select(x => x.Name).ToList()
                });
            }
            return trackResponse;
        }

        #endregion
    }
}
