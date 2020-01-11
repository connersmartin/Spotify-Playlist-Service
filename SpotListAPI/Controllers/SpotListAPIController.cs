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
        public async Task<List<PlaylistResponse>> GetPlaylists([FromBody] PlaylistRequest request)
        {            
            request.Auth = HttpContext.Request.Headers["auth"];

            var playlists = await _playlistService.GetPlaylists(request);

            //return id of playlist and playlist name/length
            var jsonPlaylists = JsonSerializer.Serialize(playlists);

            return playlists;
        }

        [HttpGet]
        [Route("Tracks")]
        public async Task<List<TrackResponse>> GetPlaylistTracks([FromBody] GetPlaylistTracksRequest request)
        {
            request.Auth = HttpContext.Request.Headers["auth"];
            var tracks = await _trackService.GetTracksFromPlaylist(request);

            var jsonTracks = JsonSerializer.Serialize(tracks);
            //return song title/artist and time
            return tracks;
        }

        [HttpPost]
        [Route("Create")]
        public async Task<PlaylistResponse> CreatePlaylist([FromBody] PlaylistRequest request)
        {          
            request.Auth = HttpContext.Request.Headers["auth"];
            
            var playList = await _playlistService.CreatePlaylist(request);
            //return playlist link
            return playList;
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
