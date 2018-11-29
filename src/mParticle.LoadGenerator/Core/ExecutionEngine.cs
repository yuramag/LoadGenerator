using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using mParticle.LoadGenerator.Services;
using Microsoft.Extensions.Options;
using mParticle.LoadGenerator.Extensions;
using mParticle.LoadGenerator.Models;

namespace mParticle.LoadGenerator.Core
{
    public sealed class ExecutionEngine : IExecutionEngine
    {
        private readonly AppSettings _settings;
        private readonly IServerApi _serverApi;
        private readonly TimeSpan _delayTime = TimeSpan.FromSeconds(1);

        public ExecutionEngine(IOptions<AppSettings> settings, IServerApi serverApi)
        {
            _settings = settings.Value;
            _serverApi = serverApi;
        }

        public IExecutionResult Execute()
        {
            var cts = new CancellationTokenSource();
            var subject = new Subject<PerfMetrics>();
            var completion = InternalRunAsync(subject.AsObserver(), cts.Token);

            return new ExecutionResult(subject.Publish().RefCount(), completion, cts);
        }

        private async Task InternalRunAsync(IObserver<PerfMetrics> output, CancellationToken cancellationToken)
        {
            var stopwatch = new Stopwatch();
            var requestsSent = 0;

            try
            {
                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var metrics = await RunCycleAsync(stopwatch, requestsSent, cancellationToken);
                    requestsSent += _settings.TargetRps;
                    output.OnNext(metrics);
                }
            }
            catch (OperationCanceledException)
            {
                output.OnCompleted();
            }
            catch (Exception e)
            {
                output.OnError(e);
            }
        }

        private async Task<PerfMetrics> RunCycleAsync(Stopwatch stopwatch, int requestsSent, CancellationToken cancellationToken)
        {
            var metrics = new PerfMetrics
            {
                TargetRps = _settings.TargetRps
            };

            var tasks = Enumerable
                .Range(requestsSent, _settings.TargetRps)
                .Select(x => _serverApi.SafeExecuteAsync(_settings.AppName, x, cancellationToken))
                .ToList();

            var batch = Task.WhenAll(tasks);

            var delay = Task.Delay(_delayTime, cancellationToken);

            var winningTask = await Task.WhenAny(batch, delay);

            if (winningTask == delay)
            {
                stopwatch.Restart();
                metrics.CurrentRps = tasks.Count(x => x.IsCompleted);
                await batch;
                metrics.TimeOffset = stopwatch.Elapsed;
            }
            else
            {
                stopwatch.Restart();
                metrics.CurrentRps = metrics.TargetRps;
                await delay;
                metrics.TimeOffset = -stopwatch.Elapsed;
            }

            metrics.Errors = tasks.Where(x => !x.Result.Succeeded).Select(x => x.Result.Error).ToList();

            return metrics;
        }
    }
}