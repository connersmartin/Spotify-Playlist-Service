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
        private readonly PlaylistService _playlistService;
        private readonly TrackService _trackService;

        public SpotListAPIController(ILogger<SpotListAPIController> logger,
                                    PlaylistService playlistService,
                                    TrackService trackService)
        {
            _logger = logger;
            _playlistService = playlistService;
            _trackService = trackService;
        }

        [HttpGet]
        [Route("Playlist")]
        public JsonResult GetPlaylists([FromBody] PlaylistRequest request)
        {            
            request.Auth = HttpContext.Request.Headers["auth"];

            var playlists = _playlistService.GetPlaylists(request);
            
            //return id of playlist and playlist name/length
            return new JsonResult("");
        }

        [HttpGet]
        [Route("Tracks")]
        public JsonResult GetPlaylistTracks([FromBody] GetPlaylistTracksRequest request)
        {
            request.Auth = HttpContext.Request.Headers["auth"];
            var tracks = _trackService.GetTracksFromPlaylist(request);
            //return song title/artist and time
            return new JsonResult("");
        }

        [HttpPost]
        [Route("Create")]
        public JsonResult CreatePlaylist([FromBody] PlaylistRequest request)
        {          
            request.Auth = HttpContext.Request.Headers["auth"];
            var playList = _playlistService.CreatePlaylist(request);
            //return playlist link
            var interim = JsonSerializer.Serialize(playList);
            return new JsonResult(interim);
        }

        [HttpPut]
        [Route("Edit")]
        public JsonResult UpdatePlaylist([FromBody] PlaylistRequest request)
        {
            request.Auth = HttpContext.Request.Headers["auth"];
            //return successful edit title
            return new JsonResult("");
        }       
    }
}
