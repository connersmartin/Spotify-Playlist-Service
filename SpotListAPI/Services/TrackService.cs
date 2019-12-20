using System;
using System.Collections.Generic;
using System.Linq;
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
        public void AddTracksToPlaylist(PlaylistRequest playlistRequest)
        {
            //Parse the params to json

            //get the recomendations

            //add the tracks
        }

        public async Task<TrackResponse> GetTracksFromPlaylist(GetPlaylistTracksRequest playlistTracksRequest)
        {
            var url = string.Format("playlists/{0}/tracks", playlistTracksRequest.Id);
            //get tracks from spotify
            var trackResponse = await _spotifyService.SpotifyApi(playlistTracksRequest.Auth, url, "get");

            //parse the tracks
            var tracks = _helper.Mapper<TrackResponse>(await trackResponse.Content.ReadAsByteArrayAsync());

            return tracks;
        }
    }
}
