using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using mParticle.LoadGenerator;
using mParticle.LoadGenerator.Core;
using mParticle.LoadGenerator.Models;
using mParticle.LoadGenerator.Services;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class ExecutionEngineTest
    {
        private Mock<IOptions<AppSettings>> _appSettingsMock;
        private Mock<IServerApi> _successfulServerApiMock;
        private Mock<IServerApi> _failingServerApiMock;
        private Mock<IServerApi> _slowServerApiMock;

        [SetUp]
        public void Setup()
        {
            var appSettings = new AppSettings
            {
                TargetRps = 5
            };

            _appSettingsMock = new Mock<IOptions<AppSettings>>();
            _appSettingsMock
                .SetupGet(x => x.Value)
                .Returns(appSettings);

            _successfulServerApiMock = new Mock<IServerApi>();
            _successfulServerApiMock
                .Setup(x => x.ExecuteAsync(It.IsAny<RequestModel>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ResponseModel { Successful = true });

            _failingServerApiMock = new Mock<IServerApi>();
            _failingServerApiMock
                .Setup(x => x.ExecuteAsync(It.IsAny<RequestModel>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ResponseModel { Successful = false });

            _slowServerApiMock = new Mock<IServerApi>();
            _slowServerApiMock
                .Setup(x => x.ExecuteAsync(It.IsAny<RequestModel>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ResponseModel { Successful = true }, TimeSpan.FromSeconds(2));
        }

        private static async Task<PerfMetrics> GetFirstMetricAsync(IExecutionEngine engine)
        {
            var result = engine.Execute();
            var metrics = await result.Output.FirstAsync();
            result.Cancel();
            await result.Completion;
            return metrics;
        }

        [Test]
        public async Task SuccessfulApiCallsShouldReturnPositiveResults()
        {
            var subject = new ExecutionEngine(_appSettingsMock.Object, _successfulServerApiMock.Object);

            var metrics = await GetFirstMetricAsync(subject);

            Assert.AreEqual(metrics.CurrentRps,  metrics.TargetRps);
            Assert.Zero(metrics.Errors.Count);
        }

        [Test]
        public async Task FailingApiCallsShouldReturnNegativeResults()
        {
            var subject = new ExecutionEngine(_appSettingsMock.Object, _failingServerApiMock.Object);

            var metrics = await GetFirstMetricAsync(subject);

            Assert.AreEqual(metrics.CurrentRps, metrics.TargetRps);
            Assert.NotZero(metrics.Errors.Count);
        }

        [Test]
        public async Task SlowApiCallsShouldReturnLowerRps()
        {
            var subject = new ExecutionEngine(_appSettingsMock.Object, _slowServerApiMock.Object);

            var metrics = await GetFirstMetricAsync(subject);

            Assert.Less(metrics.CurrentRps, metrics.TargetRps);
        }
    }
}
