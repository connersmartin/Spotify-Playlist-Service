using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotListAPI.Models
{
    public class PaginatedPlaylistTrackResponse:BasePaginatedObject
    {
        public PlaylistTrack[] items {get;set;}
    }
}
