using System;
using System.Collections.Generic;
using System.Linq;
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

        public PlaylistService (ILogger<PlaylistService> logger, UserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        //create/list playlist url users/{user_id}/playlists
        public async Task<PlaylistResponse> CreatePlaylist(PlaylistRequest playlistRequest)
        {
            var userId = await _userService.GetUser(playlistRequest.Auth);

            return new PlaylistResponse();
        }
        //update playlist details url playlists/{playlist_id}
    }
}
