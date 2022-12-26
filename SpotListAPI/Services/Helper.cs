using SpotListAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SpotListAPI.Services
{
    public class Helper
    {
        //General mapper
        public T Mapper<T>(byte[] json)
        {
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions());
        }
        public List<List<T>> ChunkBy<T>(List<T> source, int chunkSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }
    }
}
