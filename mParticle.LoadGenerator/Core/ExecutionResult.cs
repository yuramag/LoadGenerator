using System;
using System.Threading;
using System.Threading.Tasks;
using mParticle.LoadGenerator.Models;
using mParticle.LoadGenerator.Services;

namespace mParticle.LoadGenerator.Core
{
    public sealed class ExecutionResult : IExecutionResult
    {
        private readonly CancellationTokenSource _cts;

        internal ExecutionResult(IObservable<PerfMetrics> output, Task completion, CancellationTokenSource cts)
        {
            Output = output;
            Completion = completion;
            _cts = cts;
        }

        public IObservable<PerfMetrics> Output { get; }

        public Task Completion { get; }

        public void Cancel()
        {
            _cts.Cancel();
        }
    }
}