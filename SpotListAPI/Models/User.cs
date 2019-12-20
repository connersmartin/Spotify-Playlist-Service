using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text;
using System.Text.Json.Serialization;

namespace SpotListAPI.Models
{
    public class User:BaseSpotifyObject
    {
        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [JsonPropertyName("followers")]
        public Follower Followers { get; set; }
        [JsonPropertyName("images")]
        public Image[] Images { get; set; }

    }
}
