﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotListAPI.Models
{
    public class PaginatedPlaylistResponse:BasePaginatedObject
    {
        public Playlist[] items { get; set; }
    }
}
