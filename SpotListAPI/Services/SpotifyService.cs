using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net.Http;
using System.Net;

namespace SpotListAPI.Services
{
    public class SpotifyService
    {
        const string baseAddress = "https://api.spotify.com/v1/";
        //API call to spotify
        public async Task<HttpResponseMessage> SpotifyApi(string auth, string url, string param, string method)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", auth);
                    client.DefaultRequestHeaders.Add("Content-Type", "application/json");
                    client.BaseAddress = new Uri(baseAddress);
                    var content = new StringContent(param);

                    switch (method)
                    {
                        case "Get":
                            return await client.GetAsync(url);
                        case "Post":
                            return await client.PostAsync(url, content);
                        case "Put":
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
