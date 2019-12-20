using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SpotListAPI.Models
{
    public class Track:BaseSpotifyObject
    {
        [JsonPropertyName("album")]
        public Album Album { get; set; }
        [JsonPropertyName("artists")]
        public Artist[] Artists { get; set; }
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
        [JsonPropertyName("is_playable")]
        public bool IsPlayable { get; set; }
        [JsonPropertyName("linked_from")]
        public BaseSpotifyObject LinkedFrom { get; set; }
        [JsonPropertyName("restrictions")]
        public Dictionary<string,string> Restrictions { get; set; }
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