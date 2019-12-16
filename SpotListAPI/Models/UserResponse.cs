using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text;

namespace SpotListAPI.Models
{
    public class UserResponse
    {
        public string Country { get; set; }
        public string Display_Name { get; set; }
        public string Id { get; set; }
        public string Email { get; set; }
        public Dictionary<string, string> External_Urls { get; set; }
        public Dictionary<string,string> Followers { get; set; }
        public string Href { get; set; }
        public Dictionary<string,string>[] Images { get; set; }
        public string Product { get; set;}
        public string Type { get; set; }
        public string Uri { get; set; }

        public static UserResponse Map (string json)
        {
            var byteString = Encoding.ASCII.GetBytes(json);
            return JsonSerializer.Deserialize<UserResponse>(byteString,null);
        }
    }
}
