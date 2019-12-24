using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotListAPI.Models
{
    public class PaginatedTrackResponse:BasePaginatedObject
    {
        public Track[] items { get; set; }

    }
}
