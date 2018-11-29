using System;
using System.Threading;
using System.Threading.Tasks;
using mParticle.LoadGenerator.Config;
using mParticle.LoadGenerator.Models;

namespace mParticle.LoadGenerator.Core
{
    public static class ServerApiExtensions
    {
        public static async Task<bool> ExecuteAsync(this IServerApi server, int requestsSent, CancellationToken cancellationToken = default)
        {
            var request = new RequestModel
            {
                Name = AppConstants.AppName,
                Date = DateTime.UtcNow,
                RequestsSent = requestsSent
            };

            var result = await server.ExecuteAsync(request, cancellationToken);

            return result.Successful;
        }

        public static async Task<OperationResult> SafeExecuteAsync(this IServerApi server, int requestsSent, CancellationToken cancellationToken = default)
        {
            try
            {
                return await server.ExecuteAsync(requestsSent, cancellationToken)
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