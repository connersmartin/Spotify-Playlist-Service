using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotListAPI.Models
{
    public class BasePaginatedObject
    {
        public string href { get; set; }
        public int limit { get; set; }
        public int? next { get; set; }
        public int offset { get; set; }
        public int? previous { get; set; }
        public int total { get; set; }
    }
}
