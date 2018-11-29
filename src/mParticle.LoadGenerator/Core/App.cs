using System;
using System.Threading.Tasks;
using mParticle.LoadGenerator.Services;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

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
                x => Console.WriteLine(JsonConvert.SerializeObject(x)),
                ex => Console.WriteLine($"ERROR: {ex}"),
                () => Console.WriteLine("COMPLETED"));

            await result.Completion;
        }
    }
}