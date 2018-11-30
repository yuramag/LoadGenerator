using System;

namespace mParticle.LoadGenerator.Models
{
    public sealed class PerfMetricsAggregate
    {
        public int TotalCycles { get; set; }
        public int TotalRequests { get; set; }
        public int TargetRps { get; set; }
        public int TotalRps { get; set; }
        public int ErrorCount { get; set; }
        public TimeSpan TotalTimeOffset { get; set; }
    }
}