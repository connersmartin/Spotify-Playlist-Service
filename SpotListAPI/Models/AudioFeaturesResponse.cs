using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SpotListAPI.Models
{
    public class AudioFeaturesResponse
    {
        [JsonPropertyName("audio_features")]
        public List<AudioFeatures> AudioFeatures { get; set; }
    }
}
