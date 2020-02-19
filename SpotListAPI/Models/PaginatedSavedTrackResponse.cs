using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotListAPI.Models
{
    public class PaginatedSavedTrackResponse:BasePaginatedObject
    {
        [JsonProperty("items")]
        public SavedTrackResponse[] Items { get; set; }
    }
}
