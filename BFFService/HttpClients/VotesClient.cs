using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BFFService.Models;

namespace BFFService.HttpClients
{
    public class VotesClient
    {
        private readonly HttpClient _client;

        public VotesClient(HttpClient httpClient)
        {
            httpClient.BaseAddress = new Uri("https://localhost:5001/votes/");
            _client = httpClient;
        }

        public Task<HttpResponseMessage> VoteAnonymouslyAsync(string phoneName, AnonymousVoteRequestDTO voteRequest)
        {
            return _client.PostAsync(
                phoneName,
                new StringContent(JsonSerializer.Serialize(voteRequest), Encoding.UTF8, "application/json")
            );
        }
        public Task<HttpResponseMessage> GetUserAsync(string id)
        {
            return _client.GetAsync(id);
        }
        public Task<HttpResponseMessage> CountVotesAsync()
        {
            return _client.PostAsync("Count", new StringContent(""));
        }
        public Task<HttpResponseMessage> GetVoteResultAsync()
        {
            return _client.GetAsync("");
        }
        public Task<HttpResponseMessage> DrawPrizesAsync(int totalPrizes, int fromTopN)
        {
            return _client.PostAsync(
                $"Draw?totalPrizes={totalPrizes}&fromTopN={fromTopN}",
                new StringContent("")
            );
        }
    }
}