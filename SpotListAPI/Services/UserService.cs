using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace SpotListAPI.Services
{
    public class UserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly SpotifyService _spotifyService;

        public UserService (ILogger<UserService> logger, SpotifyService spotifyService)
        {
            _logger = logger;
            _spotifyService = spotifyService;
        }
        public async Task<string> GetUser(string auth)
        {
            var url = "me";
            var getUserResponse = await _spotifyService.SpotifyApi(auth, url, "get");
            var interim = await getUserResponse.Content.ReadAsStringAsync();
            //deserialize somehow
            return interim;
        }
    }
}
