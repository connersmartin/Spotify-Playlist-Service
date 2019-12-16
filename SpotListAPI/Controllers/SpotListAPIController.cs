using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SpotListAPI.Services;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using SpotListAPI.Models;

namespace SpotListAPI.Controllers
{
    [ApiController]
    [Route("SpotListAPI")]
    public class SpotListAPIController : ControllerBase
    {

        private readonly ILogger<SpotListAPIController> _logger;
        private readonly UserService _userService;

        public SpotListAPIController(ILogger<SpotListAPIController> logger, UserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpGet]
        [Route("Playlist")]
        public JsonResult GetPlaylists([FromBody] GetPlaylistRequest request)
        {
            var auth = HttpContext.Request.Headers["auth"];
            var user = _userService.GetUser(auth);

            //return id of playlist and playlist name/length
            return new JsonResult("");
        }

        [HttpGet]
        [Route("Tracks")]
        public JsonResult GetPlaylistTracks([FromBody] GetPlaylistTracksRequest request)
        {
            var auth = HttpContext.Request.Headers["auth"];
            var user = _userService.GetUser(auth);

            //return song title/artist and time
            return new JsonResult("");
        }

        [HttpPost]
        [Route("Create")]
        public async Task<JsonResult> CreatePlaylist([FromBody] PlaylistRequest request)
        {
            var auth = HttpContext.Request.Headers["auth"];
            var user = await _userService.GetUser(auth);

            //return playlist link
            return new JsonResult("");
        }

        [HttpPut]
        [Route("Edit")]
        public JsonResult UpdatePlaylist([FromBody] PlaylistRequest request)
        {
            var auth = HttpContext.Request.Headers["auth"];
            //return successful edit title
            return new JsonResult("");
        }       
    }
}
