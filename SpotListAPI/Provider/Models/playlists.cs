using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotListAPI.Provider.Models
{
    public class playlists
    {
        public ObjectId id { get; set; }
        public string userId { get; set; }
        public string playListId { get; set; }
    }
}
