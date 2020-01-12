using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SpotListAPI.Models
{
    public class PlaylistTrack
    {
        [JsonPropertyName("added_at")]
        public DateTime AddedAt { get; set; }
        [JsonPropertyName("added_by")]
        public User AddedBy { get; set; }
        [JsonPropertyName("is_local")]
        public bool IsLocal { get; set; }
        [JsonPropertyName("track")]
        public Track Track { get; set; }
        [JsonPropertyName("primary_color")]
        public string PrimaryColor { get; set; }
        [JsonPropertyName("video_thumbnail")]
        public Dictionary<string,string> VideoThumbnail { get; set; }

    }

}
