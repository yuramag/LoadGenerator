using System;
using System.Threading.Tasks;
using mParticle.LoadGenerator.Models;

namespace mParticle.LoadGenerator.Services
{
    public interface IExecutionResult
    {
        IObservable<PerfMetrics> Output { get; }
        Task Completion { get; }
        void Cancel();
    }
}