using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using RouterLogIngestion.Models;

namespace RouterLogIngestion.Functions;

public class GeocodeIpAddressBatchActivity
{
    private readonly ILogger<GeocodeIpAddressBatchActivity> _logger;
    private readonly HttpClient _httpClient;
    private readonly TelemetryClient _telemetryClient;

    public GeocodeIpAddressBatchActivity(ILogger<GeocodeIpAddressBatchActivity> logger,
        IHttpClientFactory httpClientFactory,
        TelemetryClient telemetryClient)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpClient = httpClientFactory.CreateClient();
        _telemetryClient = telemetryClient ?? throw new ArgumentNullException(nameof(telemetryClient));
    }

    [FunctionName(nameof(GeocodeIpAddressBatchActivity))]
    public async Task GeocodeIpAddressBatch([ActivityTrigger] List<IpTablesLogEntry> logEntry,
        [EventHub("logs", Connection = "eventHubConnectionString")] IAsyncCollector<MondoBase> data)
    {
        try
        {
            var uri = new Uri("http://ip-api.com/batch/json/"); // TODO: move to config
            var batch = logEntry.Select(_ => new GeolocationQuery(_.Src));
            var bodyString = JsonConvert.SerializeObject(batch);
            var bodyBytes = Encoding.UTF8.GetBytes(bodyString);
            var bodyContent = new ByteArrayContent(bodyBytes);
            bodyContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = await _httpClient.PostAsync(uri, bodyContent);
            var responseString = await response.Content.ReadAsStringAsync();
            var body = JsonConvert.DeserializeObject<IEnumerable<GeocodeResult>>(responseString);

            var @event = new EventTelemetry("GeocodeBatchCompleted");
            var counter = 1;

            foreach (var item in body)
            {
                @event.Properties.Add($"IP Address {counter.ToString()}", item.Country);
                counter += 1;
            }

            var resultArray = body.ToArray();

            for (var i = 0; i < resultArray.Length; i++)
            {
                await data.AddAsync(new MondoBase(resultArray[i], logEntry[i]));
            }

            _telemetryClient.TrackEvent(@event);
        }
        catch (Exception e)
        {
            var et = new ExceptionTelemetry(e);
            var message = $"Exception caught while calling IP API: {e.Message}";
            et.Message = message;
            _telemetryClient.TrackException(et);
            throw new LogIngestionException(message, e);
        }
    }

    public class LogIngestionException : ApplicationException
    {
        public LogIngestionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}