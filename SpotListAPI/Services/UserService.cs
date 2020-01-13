using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpotListAPI.Models;

namespace SpotListAPI.Services
{
    public class UserService
    {
        private readonly ILogger _logger;
        private readonly SpotifyService _spotifyService;
        private readonly Helper _helper;
        public UserService(ILogger<UserService> logger, SpotifyService spotifyService, Helper helper)
        {
            _logger = logger;
            _spotifyService = spotifyService;
            _helper = helper;
        }
        //Gets userid of current user
        public async Task<string> GetUser(string auth)
        {
            var url = "me";
            var userResponse = await _spotifyService.SpotifyApi(auth, url,"get");
            var userResponseString = await userResponse.Content.ReadAsByteArrayAsync();
            var user = _helper.Mapper<User>(userResponseString);

            return user.Id;
        }
    }
}
