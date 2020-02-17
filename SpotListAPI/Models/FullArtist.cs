using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SpotListAPI.Models
{
    public class FullArtist:BaseSpotifyObject
    {
        [JsonPropertyName("popularity")]
        public int Popularity { get; set; }
        [JsonPropertyName("images")]
        public Image[] Images { get; set; }
        [JsonPropertyName("genres")]
        public string[] Genres { get; set; }
        [JsonPropertyName("followers")]
        public Follower Followers { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }

    }
}
