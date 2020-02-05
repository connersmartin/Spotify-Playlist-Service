using System.Text.Json.Serialization;

namespace SpotListAPI.Models
{
    public class AudioFeatures
    {
        [JsonPropertyName("acousticness")]
        public float Acousticness { get; set; }
        [JsonPropertyName("analysis_url")]
        public string AnalysisUrl { get; set; }
        [JsonPropertyName("danceability")]
        public float Danceability { get; set; }
        [JsonPropertyName("duration_ms")]
        public int DurationMs { get; set; }
        [JsonPropertyName("energy")]
        public float Energy { get; set; }
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("instrumentalness")]
        public float Instrumentalness { get; set; }
        [JsonPropertyName("key")]
        public int Key { get; set; }
        [JsonPropertyName("liveness")]
        public float Liveness { get; set; }
        [JsonPropertyName("loudness")]
        public float Loudness { get; set; }
        [JsonPropertyName("mode")]
        public int Mode { get; set; }
        [JsonPropertyName("speechiness")]
        public float Speechiness { get; set; }
        [JsonPropertyName("tempo")]
        public float Tempo { get; set; }
        [JsonPropertyName("time_signature")]
        public int TimeSignature { get; set; }
        [JsonPropertyName("track_href")]
        public string TrackHref { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("uri")]
        public string Uri { get; set; }
        [JsonPropertyName("valence")]
        public float Valence { get; set; }

    }
}