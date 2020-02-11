using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotListAPI.Models
{
    public class PaginatedSavedTrackResponse:BasePaginatedObject
    {
        public SavedTrackResponse[] Items { get; set; }
    }
}
