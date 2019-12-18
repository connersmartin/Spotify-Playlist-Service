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
        public string ParamsToJson(Dictionary<string,string> paramDict)
        {

            return "";
        }

        public T Mapper<T>(byte[] json)
        {
            return JsonSerializer.Deserialize<T>(json, null);
        } 
    }
}
