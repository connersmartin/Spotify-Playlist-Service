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
        public async Task<List<PlaylistResponse>> GetPlaylists(string id = null)
        {
            //cache this
            var request = new PlaylistRequest()
            {
                Auth = HttpContext.Request.Headers["auth"],
                UpdateTracks = true,
                Id = id
            };
            if (id != null)
            {
                var playlist = await _playlistService.GetPlaylist(request);

                return playlist;
            }
            else
            {
                var playlists = await _playlistService.GetPlaylists(request);

                return playlists;
            }
           
            
            //return id of playlist and playlist name/length
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

            /*var playList = new PlaylistResponse()
            {
                Id = "Test",
                Length = request.Length,
                Title = request.Name,
                TrackCount = 0
            };*/

            //return playlist link
            return playList;
        }

        [HttpPost]
        [Route("Update")]
        public async Task<JsonResult> UpdateSavedTracksPlaylist(string id)
        {
            //Would want to use the unfollow option
            var request = new PlaylistRequest()
            {
                Auth = HttpContext.Request.Headers["auth"],
                Id = id,
                UpdateTracks = true,
                SavedTracks = true
            };

            var response = await _playlistService.UpdatePlaylist(request);
            //return successful edit title
            return new JsonResult(response.TrackCount);
        } 
        
        [Route("Copy")]
        public async Task<JsonResult> CopyTracksPlaylist(string id)
        {
            //Would want to use the unfollow option
            var request = new PlaylistRequest()
            {
                Auth = HttpContext.Request.Headers["auth"],
                Id = id,
                UpdateTracks = true,
                SavedTracks = true
            };

            var response = await _playlistService.CopyPlaylist(request);
            //return successful edit title
            return new JsonResult(response);
        }

        [HttpGet]
        [Route("Delete")]
        public JsonResult DeletePlaylist(string id)
        {
            //Would want to use the unfollow option
            var request = new PlaylistRequest()
            {
                Auth = HttpContext.Request.Headers["auth"],
                Id = id
            };    

            var unfollowResponse = _playlistService.UnfollowPlaylist(request);
            //return successful edit title
            return new JsonResult(unfollowResponse);
        }       
    }
}
