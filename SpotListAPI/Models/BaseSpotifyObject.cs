using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SpotListAPI.Models
{
    public class BaseSpotifyObject
    {
        [JsonPropertyName("href")]
        public string Href { get; set; }
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("images")]
        public Image[] Images { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("uri")]
        public string Uri { get; set; }
        [JsonPropertyName("external_urls")]
        public Dictionary<string, string> ExternalUrls { get; set; }
    }
}
