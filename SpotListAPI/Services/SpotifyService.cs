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
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer "+ auth);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.BaseAddress = new Uri(baseAddress);
                    var content = new StringContent(param);

                    switch (method.ToLower())
                    {
                        case "get":
                            return await client.GetAsync(baseAddress+url);
                        case "post":
                            return await client.PostAsync(url, content);
                        case "put":
                            return await client.PutAsync(url, content);
                    }
                }
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.NoContent };
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.InternalServerError, ReasonPhrase = ex.Message };
            }

        }
    }
}
