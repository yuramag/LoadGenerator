using System.IO;
using System.Threading.Tasks;
using mParticle.LoadGenerator.Core;
using mParticle.LoadGenerator.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace mParticle.LoadGenerator
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();

            ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            await serviceProvider.GetService<App>().RunAsync();
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false)
                .AddEnvironmentVariables()
                .Build();

            serviceCollection.AddLogging(builder => builder
                .AddConsole()
                .AddDebug());

            serviceCollection.AddOptions();

            serviceCollection.Configure<AppSettings>(configuration.GetSection(nameof(AppSettings)));

            serviceCollection.AddTransient<IServerApi, ServerApi>();

            serviceCollection.AddTransient<App>();
        }
    }
}
