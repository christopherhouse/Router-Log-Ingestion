using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RouterLogIngestion.Models;

namespace RouterLogIngestion.Functions.Entities;

[JsonObject(MemberSerialization.OptIn)]
public class IpGeocodeCache
{
    private readonly TelemetryClient _telemetryClient;
    private readonly ILogger<IpGeocodeCache> _logger;

    public IpGeocodeCache(TelemetryClient telemetryClient, ILogger<IpGeocodeCache> logger)
    {
        _telemetryClient = telemetryClient ?? throw new ArgumentNullException(nameof(telemetryClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        Cache = new Dictionary<string, GeocodeResult>(1000);
    }

    [JsonProperty("cache")]
    public Dictionary<string, GeocodeResult> Cache { get; }

    public void Add(GeocodeResult resultToCache)
    {
        if (Cache.Keys.Count > 1000)
        {
            var countToRemove = Cache.Keys.Count - 1000;
            var keysToRemove = new List<string>();

            for (var i = 0; i < countToRemove; i++)
            {
                keysToRemove.Add(Cache.Keys.ElementAt(i));
            }

            foreach (var key in keysToRemove)
            {
                Cache.Remove(key);
            }
        }

        Cache.Add(resultToCache.SourceIpAddress, resultToCache);
    }


    [FunctionName(nameof(IpGeocodeCache))]
    public static Task Run([EntityTrigger] IDurableEntityContext context) => context.DispatchAsync<IpGeocodeCache>();
}