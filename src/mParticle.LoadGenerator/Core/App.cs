using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using mParticle.LoadGenerator.Extensions;
using mParticle.LoadGenerator.Models;
using mParticle.LoadGenerator.Services;
using Microsoft.Extensions.Options;

namespace mParticle.LoadGenerator.Core
{
    public sealed class App
    {
        private readonly AppSettings _settings;
        private readonly IExecutionEngine _engine;

        public App(IOptions<AppSettings> settings, IExecutionEngine engine)
        {
            _settings = settings.Value;
            _engine = engine;
        }

        public async Task RunAsync()
        {
            Console.WriteLine($"Running App: {_settings.Title}");

            var result = _engine.Execute();

            Console.CancelKeyPress += (sender, args) =>
            {
                args.Cancel = true;
                result.Cancel();
            };

            result.Output.Subscribe(
                x => Console.WriteLine(x.Print()),
                ex => Console.WriteLine($"ERROR: {ex}"),
                () => Console.WriteLine("COMPLETED"));

            result.Output.Aggregate(new PerfMetricsAggregate(), (aggregate, metrics) =>
            {
                aggregate.TotalCycles += 1;
                aggregate.TotalRequests += metrics.TargetRps;
                aggregate.TargetRps = metrics.TargetRps;
                aggregate.TotalRps += metrics.CurrentRps;
                aggregate.ErrorCount += metrics.Errors.Count;
                aggregate.TotalTimeOffset += metrics.TimeOffset;
                return aggregate;
            }).Subscribe(x =>
            {
                Console.WriteLine("\nSUMMARY:");
                Console.WriteLine(x.Print());
            });

            await result.Completion;
        }
    }
}