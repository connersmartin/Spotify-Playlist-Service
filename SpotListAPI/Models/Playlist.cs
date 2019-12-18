using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SpotListAPI.Models
{
    public class Playlist:BaseSpotifyObject
    {
        [JsonPropertyName("collaborative")]
        public bool Collaborative { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("href")]
        public string Name { get; set; }
        [JsonPropertyName("owner")]
        public User Owner { get; set; }
        [JsonPropertyName("public")]
        public Boolean? Public { get; set; }
        [JsonPropertyName("snapshot_id")]
        public string SnapshotId { get; set; }
        [JsonPropertyName("tracks")]
        public Track[] Tracks { get; set; }
        [JsonPropertyName("followers")]
        public Follower Followers { get; set; }

    }

    //    collaborative Boolean Returns true if context is not search and the owner allows other users to modify the playlist.Otherwise returns false.
    //description string The playlist description. Only returned for modified, verified playlists, otherwise null.
    //external_urls an external URL object Known external URLs for this playlist.
    //followers a followers object Information about the followers of the playlist.
    //href    string A link to the Web API endpoint providing full details of the playlist.
    //id  string The Spotify ID for the playlist.
    //images an array of image objects   Images for the playlist. The array may be empty or contain up to three images.The images are returned by size in descending order. See Working with Playlists.Note: If returned, the source URL for the image (url ) is temporary and will expire in less than a day.
    //name    string The name of the playlist.
    //owner a public user object The user who owns the playlist
    //public Boolean or null	The playlist’s public/private status: true the playlist is public, false the playlist is private, null the playlist status is not relevant.For more about public/private status, see Working with Playlists.
    //snapshot_id string The version identifier for the current playlist.Can be supplied in other requests to target a specific playlist version: see Remove tracks from a playlist
    //tracks array of playlist track objects inside a paging object Information about the tracks of the playlist.
    //type    string The object type: “playlist”
    //uri string The Spotify URI for the playlist.
}
