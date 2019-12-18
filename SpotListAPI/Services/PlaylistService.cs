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
            _trackService.AddTracksToPlaylist(playlistRequest);

            return new PlaylistResponse();
        }
        //update playlist details url playlists/{playlist_id}

        public async Task<string> AddBlankPlaylist(PlaylistRequest playlistRequest)
        {
            var url = string.Format("users/{0}/playlists", playlistRequest.UserId);
            var jsonParams = JsonSerializer.Serialize(new KeyValuePair<string,string>("name",playlistRequest.Name));
            var addPlaylistResponse = await _spotifyService.SpotifyApi(playlistRequest.Auth, url, "post", jsonParams);            
            //return the id
            var playlist = _helper.Mapper<PlaylistResponse>(await addPlaylistResponse.Content.ReadAsByteArrayAsync());
            
            return playlist.Id;
        }
    }
}
