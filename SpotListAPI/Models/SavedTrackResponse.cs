using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotListAPI.Models
{
    public class SavedTrackResponse
    {
        [JsonProperty("added_at")]
        public DateTime AddedAt { get; set; }
        [JsonProperty("track")]
        public SavedTrack Track { get; set; }

    }
}
