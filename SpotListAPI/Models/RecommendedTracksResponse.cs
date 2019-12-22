using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotListAPI.Models
{
    public class RecommendedTracksResponse
    {
        public Track[] tracks { get; set; }
        public Seeds[] seeds { get; set; }
    }
}
