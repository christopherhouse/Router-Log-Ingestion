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
            // TODO:  Add some smarts here, don't add a source IP to the queue more than once.  Hold duplicates back somehow?  Another entity?  Yeah, another entity :D
            GeocodeBatchQueue.Add(logEntry);

            _logger.LogInformation($"Added FW block from {logEntry.Src} to queue");
            _telemetryClient.TrackEvent("QueueItemAdded", new Dictionary<string, string>(){{"Source", logEntry.Src}, {"DestPort", logEntry.Dpt}, {"QueueDepth", GeocodeBatchQueue.Count.ToString()}});
            
            if (GeocodeBatchQueue.Count > 99) // TODO: Make queue depth configurable
            {
                _telemetryClient.TrackEvent("GeocodeBatchCreated");
                var batchToRun = new List<IpTablesLogEntry>();
                var counter = 0;

                while (counter < 100)
                {
                    var itemToAdd = GeocodeBatchQueue[counter];
                    batchToRun.Add(itemToAdd);
                    counter += 1;
                }

                GeocodeBatchQueue.RemoveRange(0, 100);


                Entity.Current.StartNewOrchestration(nameof(GeocodeIpAddressBatchOrchestration), batchToRun);
            }
        }

        [FunctionName(nameof(IpGeocodeQueue))]
        public static Task Run([EntityTrigger] IDurableEntityContext context) => context.DispatchAsync<IpGeocodeQueue>();
    }
}
