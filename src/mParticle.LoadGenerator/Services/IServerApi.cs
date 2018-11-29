using System;
using System.Threading;
using System.Threading.Tasks;
using mParticle.LoadGenerator.Models;

namespace mParticle.LoadGenerator.Services
{
    public interface IServerApi : IDisposable
    {
        Task<ResponseModel> ExecuteAsync(RequestModel request, CancellationToken cancellationToken = default);
    }
}