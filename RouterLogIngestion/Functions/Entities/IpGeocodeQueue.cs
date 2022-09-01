using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RouterLogIngestion.Models;

namespace RouterLogIngestion.Functions.Entities
{
    [JsonObject(MemberSerialization.OptIn)]
    public class IpGeocodeQueue
    {
        private readonly TelemetryClient _telemetryClient;
        private readonly ILogger<IpGeocodeQueue> _logger;

        public IpGeocodeQueue(TelemetryClient telemetryClient, 
            ILogger<IpGeocodeQueue> logger)
        {
            _telemetryClient = telemetryClient ?? throw new ArgumentNullException(nameof(telemetryClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            GeocodeBatchQueue = new List<IpTablesLogEntry>();
        }

        [JsonProperty(nameof(GeocodeBatchQueue))]
        public List<IpTablesLogEntry> GeocodeBatchQueue { get;  }

        public void AddLogEntryToQueue(IpTablesLogEntry logEntry)
        {
            GeocodeBatchQueue.Add(logEntry);

            _logger.LogInformation($"Added FW block from {logEntry.Src} to queue");
            _telemetryClient.TrackEvent("QueueItemAdded", new Dictionary<string, string>(){{"Source", logEntry.Src}, {"DestPort", logEntry.Dpt}, {"QueueDepth", GeocodeBatchQueue.Count.ToString()}});
            
            if (GeocodeBatchQueue.Count > 100)
            {
                _telemetryClient.TrackEvent("GeocodeBatchCreated");

                Entity.Current.StartNewOrchestration(nameof(GeocodeIpAddressBatchOrchestration), GeocodeBatchQueue);

                GeocodeBatchQueue.Clear();
            }
        }

        [FunctionName(nameof(IpGeocodeQueue))]
        public static Task Run([EntityTrigger] IDurableEntityContext context) => context.DispatchAsync<IpGeocodeQueue>();
    }


}
