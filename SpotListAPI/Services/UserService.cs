using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotListAPI.Services
{
    public class UserService
    {
        private readonly ILogger _logger;
        private readonly SpotifyService _spotifyService;
        public UserService(ILogger logger, SpotifyService spotifyService)
        {
            _logger = logger;
            _spotifyService = spotifyService;
        }
        public async Task<string> GetUser(string auth)
        {
            var url = "/me";
            //need to figure out a helper to parse this response
            await _spotifyService.SpotifyApi(auth, url, null,"get");
            return "";
        }
    }
}
