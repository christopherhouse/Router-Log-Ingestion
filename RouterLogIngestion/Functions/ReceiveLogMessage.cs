using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Linq;

namespace RouterLogIngestion.Functions;

public class ReceiveLogMessage
{
    private readonly ILogger<ReceiveLogMessage> _logger;
    private readonly TelemetryClient _telemetryClient;

    public ReceiveLogMessage(ILogger<ReceiveLogMessage> log,
        TelemetryClient telemetryClient)
    {
        _logger = log;
        _telemetryClient = telemetryClient ?? throw new ArgumentNullException(nameof(telemetryClient));
    }

    [FunctionName(Constants.FunctionNames.ReceiveLogMessage)]
    [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
    [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
    [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
        [DurableClient] IDurableOrchestrationClient orchestrationClient)
    {
        IActionResult result = null;

        using var reader = new StreamReader(req.Body);
        var bodyString = await reader.ReadToEndAsync();
        var json = JObject.Parse(bodyString);
        const string fwObjectName = "iptables";

        if (json.ContainsKey(fwObjectName))
        {
            _telemetryClient.TrackEvent("FirewallDrop");
            _logger.LogInformation("Got FW block log entry");
            var orchestrationId = await orchestrationClient.StartNewAsync(Constants.FunctionNames.ProcessLogEntryOrchestration, null, bodyString);
            result = orchestrationClient.CreateCheckStatusResponse(req, orchestrationId);
        }
        else
        {
            _telemetryClient.TrackEvent("Non-firewall event");
            var keys = string.Join(",", json.SelectTokens("$.*~").Select(_ => _.Value<string>()));
            _logger.LogInformation($"Syslog message wasn't a firewall drop, message keys are: {keys}");
            result = new OkResult();
        }

        return result;
    }
}