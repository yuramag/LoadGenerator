using System;
using System.Collections.Generic;

namespace mParticle.LoadGenerator.Models
{
    public sealed class PerfMetrics
    {
        public int TargetRps { get; set; }
        public int CurrentRps { get; set; }
        public TimeSpan TimeOffset { get; set; }
        public List<string> Errors { get; set; }
    }
}