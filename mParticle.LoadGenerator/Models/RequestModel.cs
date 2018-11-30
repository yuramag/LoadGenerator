using System;
using Newtonsoft.Json;

namespace mParticle.LoadGenerator.Models
{
    public sealed class RequestModel
    {
        public string Name { get; set; }

        public DateTime Date { get; set; }

        [JsonProperty("requests_sent")]
        public int RequestsSent { get; set; }
    }
}