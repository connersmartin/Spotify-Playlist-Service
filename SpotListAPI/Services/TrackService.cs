using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpotListAPI.Models;

namespace SpotListAPI.Services
{
    public class TrackService
    {
        //add/get tracks url playlists/{playlist_id}/tracks
        public void AddTracksToPlaylist(PlaylistRequest playlistRequest)
        {
            //Parse the params to json

            //get the recomendations

            //add the tracks
        }

        public async Task<TrackResponse> GetTracksFromPlaylist(PlaylistRequest playlistRequest)
        {
            //get tracks from spotify

            //parse the tracks
            return new TrackResponse();
        }
    }
}
