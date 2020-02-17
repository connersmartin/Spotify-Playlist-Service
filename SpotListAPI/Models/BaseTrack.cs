using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SpotListAPI.Models
{
    public class BaseTrack:BaseSpotifyObject
    {
       
        [JsonPropertyName("available_markets")]
        public string[] AvailableMarkets { get; set; }
        [JsonPropertyName("disc_number")]
        public int DiscNumber { get; set; }
        [JsonPropertyName("duration_ms")]
        public int DurationMs { get; set; }
        [JsonPropertyName("explicit")]
        public bool Explicit { get; set; }
        [JsonPropertyName("external_ids")]
        public Dictionary<string,string> ExternalIds { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("popularity")]
        public int Popularity { get; set; }
        [JsonPropertyName("preview_url")]
        public string PreviewUrl { get; set; }
        [JsonPropertyName("track_number")]
        public int TrackNumber { get; set; }
        [JsonPropertyName("is_local")]
        public bool IsLocal { get; set; }
    }

}