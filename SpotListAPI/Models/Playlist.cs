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
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("owner")]
        public User Owner { get; set; }
        [JsonPropertyName("public")]
        public bool? Public { get; set; }
        [JsonPropertyName("snapshot_id")]
        public string SnapshotId { get; set; }
        [JsonPropertyName("tracks")]
        public Track[] Tracks { get; set; }
        [JsonPropertyName("followers")]
        public Follower Followers { get; set; }
        [JsonPropertyName("images")]
        public Image[] Images { get; set; }

    }

}
