using System;
using System.Linq;
using mParticle.LoadGenerator.Models;

namespace mParticle.LoadGenerator.Extensions
{
    public static class HelperExtensions
    {
        private static readonly TimeSpan _oneSecond = TimeSpan.FromSeconds(1);

        public static string Print(this PerfMetrics metrics)
        {
            var isNegative = metrics.TimeOffset < TimeSpan.Zero;
            var percent = Math.Round(metrics.TimeOffset.Duration().TotalMilliseconds / _oneSecond.TotalMilliseconds * 100);
            var percentSuffix = isNegative ? "% under" : "% over";

            var errors = string.Join("\n", metrics.Errors.Select(x => $"\t{x}"));

            errors = string.IsNullOrEmpty(errors) ? null : $", Errors ({metrics.Errors.Count}):\n{errors}";

            return $"Target RPS: {metrics.TargetRps}, Current RPS: {metrics.CurrentRps}, Time Offset: {percent}{percentSuffix}{errors}";
        }

        public static string Print(this PerfMetricsAggregate metrics)
        {
            var averageRps = (double)metrics.TotalRps / metrics.TotalCycles;

            return $"Target RPS: {metrics.TargetRps}, Average RPS: {averageRps:F2}, Total Requests: {metrics.TotalRequests}, Errors: {metrics.ErrorCount}, Total Time Offset: {metrics.TotalTimeOffset}";
        }
    }
}