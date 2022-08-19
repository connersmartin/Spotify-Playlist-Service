using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SpotListAPI.Models
{
    public class SavedTrack:BaseSpotifyObject
    {
        [JsonPropertyName("album")]
        public TrackAlbum Album { get; set; }
        [JsonPropertyName("added_at")]
        public DateTime AddedAt { get; set; }
        [JsonPropertyName("artists")]
        public BaseArtist[] Artists { get; set; }
        [JsonPropertyName("available_markets")]
        public string[] AvailableMarkets { get; set; }
        [JsonPropertyName("disc_number")]
        public int? DiscNumber { get; set; }
        [JsonPropertyName("duration_ms")]
        public int DurationMs { get; set; }
        [JsonPropertyName("explicit")]
        public bool Explicit { get; set; }
        [JsonPropertyName("external_ids")]
        public Dictionary<string, string> ExternalIds { get; set; }
        [JsonPropertyName("is_local")]
        public bool IsLocal { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("popularity")]
        public int? Popularity { get; set; }
        [JsonPropertyName("preview_url")]
        public string PreviewUrl { get; set; }
        [JsonPropertyName("track_number")]
        public int? TrackNumber { get; set; }
    }
}
