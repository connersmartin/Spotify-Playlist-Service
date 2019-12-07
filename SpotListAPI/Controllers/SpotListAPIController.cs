using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SpotListAPI.Services;
using System.Text.Json;

namespace SpotListAPI.Controllers
{
    [ApiController]
    [Route("SpotListAPI")]
    public class SpotListAPIController : ControllerBase
    {

        private readonly ILogger<SpotListAPIController> _logger;

        public SpotListAPIController(ILogger<SpotListAPIController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("Playlist")]
        public JsonResult GetPlaylists([FromBody] JsonElement request)
        {
            var x = request;
            //return id of playlist and playlist name/length
            return new JsonResult("");
        }

        [HttpGet]
        [Route("Tracks")]
        public JsonResult GetPlaylistTracks([FromBody] JsonElement request)
        {
            //return song title/artist and time
            return new JsonResult("");
        }

        [HttpPost]
        [Route("Create")]
        public JsonResult CreatePlaylist([FromBody] JsonElement request)
        {
            //return playlist link
            return new JsonResult("");
        }

        [HttpPut]
        [Route("Edit")]
        public JsonResult UpdatePlaylist([FromBody] JsonElement request)
        {
            //return successful edit title
            return new JsonResult("");
        }
    }
}
