using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text;
using System.Text.Json.Serialization;

namespace SpotListAPI.Models
{
    public class UserResponse
    {
        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [JsonPropertyName("external_urls")]
        public Dictionary<string, string> ExternalUrls { get; set; }
        [JsonPropertyName("followers")]
        public Follower Followers { get; set; }
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

        public static UserResponse Map (string json)
        {
            var byteString = Encoding.ASCII.GetBytes(json);
            return JsonSerializer.Deserialize<UserResponse>(byteString,null);
        }
    }
}
