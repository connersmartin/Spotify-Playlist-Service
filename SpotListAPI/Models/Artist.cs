using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SpotListAPI.Models
{
    public class Artist:BaseSpotifyObject
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

}
