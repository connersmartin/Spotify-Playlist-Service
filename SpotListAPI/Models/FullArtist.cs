using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SpotListAPI.Models
{
    public class FullArtist:Artist
    {
        [JsonPropertyName("popularity")]
        public int Popularity { get; set; }
        [JsonPropertyName("images")]
        public Image[] Images { get; set; }
        [JsonPropertyName("genres")]
        public string[] Genres { get; set; }

    }
}
