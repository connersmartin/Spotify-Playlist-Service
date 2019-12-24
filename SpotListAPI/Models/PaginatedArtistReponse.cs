using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotListAPI.Models
{
    public class PaginatedArtistResponse:BasePaginatedObject
    {
        public Artist[] items { get; set; }

    }
}
