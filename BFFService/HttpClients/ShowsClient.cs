using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BFFService.Models;
using Microsoft.Extensions.Configuration;

namespace BFFService.HttpClients
{
    public class ShowsClient
    {
        private readonly HttpClient _client;

        public ShowsClient(HttpClient httpClient, IConfiguration config)
        {
            var bizServiceUrl = config.GetValue<string>("BizServiceUrl");
            httpClient.BaseAddress = new Uri($"{bizServiceUrl}/shows/");
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
            return _client.PutAsync(
                id, 
                new StringContent(JsonSerializer.Serialize(show), Encoding.UTF8, "application/json")
            );
        }
        
        public Task<HttpResponseMessage> DeleteShowAsync(string id)
        {
            return _client.DeleteAsync(id);
        }
    }
}