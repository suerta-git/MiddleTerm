using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BFFService.Models;

namespace BFFService.HttpClients
{
    public class ShowsClient
    {
        private readonly HttpClient _client;

        public ShowsClient(HttpClient httpClient)
        {
            httpClient.BaseAddress = new Uri("https://localhost:5001/shows/");
            _client = httpClient;
        }

        public Task<HttpResponseMessage> GetShowsAsync()
        {
            return _client.GetAsync(""); 
        }

        public Task<HttpResponseMessage> GetShowAsync(string id)
        {
            return _client.GetAsync(id);
        }

        public Task<HttpResponseMessage> AddShowAsync(Show show)
        {
            return _client.PostAsync(
                "", 
                new StringContent(JsonSerializer.Serialize(show), Encoding.UTF8, "application/json")
            );
        }
        
        public Task<HttpResponseMessage> UpdateShowAsync(string id, Show show)
        {
            return _client.PostAsync(
                id, 
                new StringContent(JsonSerializer.Serialize(show), Encoding.UTF8, "application/json")
            );
        }
    }
}