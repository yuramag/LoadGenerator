using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using mParticle.LoadGenerator.Models;
using mParticle.LoadGenerator.Services;
using Microsoft.Extensions.Options;

namespace mParticle.LoadGenerator.Core
{
    public sealed class ServerApi : IServerApi
    {
        private readonly AppSettings _settings;
        private readonly HttpClient _client;

        public ServerApi(IOptions<AppSettings> settings)
        {
            _settings = settings.Value;

            _client = new HttpClient
            {
                BaseAddress = new Uri(_settings.ServerUrl),
                DefaultRequestHeaders = {{"X-Api-Key", _settings.AuthKey}}
            };
        }

        public async Task<ResponseModel> ExecuteAsync(RequestModel request, CancellationToken cancellationToken = default)
        {
            var response = await _client.PostAsJsonAsync("Live", request, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<ResponseModel>(cancellationToken);
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}