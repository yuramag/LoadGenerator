using System;
using System.Threading;
using System.Threading.Tasks;
using mParticle.LoadGenerator.Models;
using mParticle.LoadGenerator.Services;

namespace mParticle.LoadGenerator.Extensions
{
    public static class ServerApiExtensions
    {
        public static async Task<bool> ExecuteAsync(this IServerApi server, string name, int requestsSent, CancellationToken cancellationToken = default)
        {
            var request = new RequestModel
            {
                Name = name,
                Date = DateTime.UtcNow,
                RequestsSent = requestsSent
            };

            var result = await server.ExecuteAsync(request, cancellationToken);

            return result.Successful;
        }

        public static async Task<OperationResult> SafeExecuteAsync(this IServerApi server, string name, int requestsSent, CancellationToken cancellationToken = default)
        {
            try
            {
                return await server.ExecuteAsync(name, requestsSent, cancellationToken)
                    ? OperationResult.Success()
                    : OperationResult.Failed("Server call was unsuccessful");
            }
            catch (Exception e)
            {
                return OperationResult.Failed(e.Message);
            }
        }
    }
}