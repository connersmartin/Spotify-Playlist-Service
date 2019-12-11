using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotListAPI.Models
{
    public class PlaylistRequest : BaseRequest
    {
        public string Auth { get; set; }
        public string Name { get; set; }
        public int Length { get; set; }
        public string[] Genres { get; set; }
        public string Artist { get; set; }
        public decimal Tempo { get; set; }
        public decimal Dance { get; set; }
        public decimal Energy { get; set; }
        public decimal Instrumental { get; set; }
    }
}
