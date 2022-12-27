using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;

namespace SpotListAPI.Services
{
    public class SpotifyService
    {
        const string baseAddress = "https://api.spotify.com/v1/";
        //API call to spotify
        public async Task<HttpResponseMessage> SpotifyApi(string auth, string url, string method, string param = "")
        {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer "+ auth);
                    client.Timeout = new TimeSpan(1,0,0);
                    client.BaseAddress = new Uri(baseAddress);
                    var content = new StringContent(param);
                    var response = new HttpResponseMessage();
                    
                    switch (method.ToLower())
                    {
                        case "get":
                            response = await client.GetAsync(baseAddress+url);
                            break;
                        case "post":
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            response = await client.PostAsync(url, content);
                            break;
                        case "put":
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            response = await client.PutAsync(url, content);
                            break;
                        case "delete":
                            response = await client.DeleteAsync(baseAddress + url);
                            break;
                    }

                    response.EnsureSuccessStatusCode();
                    return response;
                }            
        }
    }
}
