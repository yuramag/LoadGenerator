using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using mParticle.LoadGenerator.Config;
using mParticle.LoadGenerator.Models;

namespace mParticle.LoadGenerator.Core
{
    public sealed class ServerApi : IServerApi
    {
        private readonly HttpClient _client;

        public ServerApi(AppSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            _client = new HttpClient
            {
                BaseAddress = new Uri(settings.ServerUrl),
                DefaultRequestHeaders = {{"X-Api-Key", settings.AuthKey}}
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