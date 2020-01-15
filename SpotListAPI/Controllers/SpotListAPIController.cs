﻿using System;
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
        public async Task<List<PlaylistResponse>> GetPlaylists()
        {
            //cache this
            var request = new PlaylistRequest()
            {
                Auth = HttpContext.Request.Headers["auth"]
            };
            
            var playlists = await _playlistService.GetPlaylists(request);
            
            //return id of playlist and playlist name/length
            return playlists;
        }

        [HttpPost]
        [Route("Tracks")]
        public async Task<List<TrackResponse>> Tracks([FromBody] GetPlaylistTracksRequest id)
        {
            var request = new GetPlaylistTracksRequest()
            {
                Auth = HttpContext.Request.Headers["auth"],
                Id = id.Id
            };
            
            var tracks = await _trackService.GetTracksFromPlaylist(request);

            //return song title/artist and time
            return _trackService.TracksToTrackResponse(tracks);
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


        //Potentially not used
        [HttpPut]
        [Route("Edit")]
        public JsonResult UpdatePlaylist([FromBody] PlaylistRequest request)
        {
            //Would want to use the unfollow option
            request.Auth = HttpContext.Request.Headers["auth"];

            var unfollowResponse = _playlistService.UnfollowPlaylist(request);
            //return successful edit title
            return new JsonResult(unfollowResponse);
        }       
    }
}
