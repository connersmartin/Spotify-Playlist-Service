using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotListAPI.Models
{
    public class PlaylistResponse : BaseResponse
    {
        public string Title { get; set; }
        public int TrackCount { get; set; }
        public int Length { get; set; }
    }
}
