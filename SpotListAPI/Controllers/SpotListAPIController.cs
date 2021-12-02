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
using SpotListAPI.Provider;
using SpotListAPI.Provider.Models;

namespace SpotListAPI.Controllers
{
    [ApiController]
    [Route("SpotListAPI")]
    public class SpotListAPIController : ControllerBase
    {

        private readonly ILogger<SpotListAPIController> _logger;
        private readonly PlaylistService _playlistService;
        private readonly TrackService _trackService;
        private readonly MongoProvider _db;

        public SpotListAPIController(ILogger<SpotListAPIController> logger,
                                    PlaylistService playlistService,
                                    TrackService trackService,
                                    MongoProvider db)
        {
            _logger = logger;
            _playlistService = playlistService;
            _trackService = trackService;
            _db = db;
        }

        [HttpGet]
        [Route("db")]
        public async Task TestDb()
        {
            //testing functionality
            var x = await _db.GetAll<playlists>("playlists");

            var b = await _db.GetOne<playlists>("playlists", "61a47031c61b8f41c7a2a809");

            var a = new
            {
                playListId = "test2",
                userId = "user2"
            };

            await _db.Add("playlists", a);
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
            try
            {
                var playlists = await _playlistService.GetPlaylists(request);

                return playlists;
            }
            catch (Exception ex)
            {

                throw ex;
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

        [HttpGet]
        [Route("Delete")]
        public JsonResult UpdatePlaylist(string id)
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
