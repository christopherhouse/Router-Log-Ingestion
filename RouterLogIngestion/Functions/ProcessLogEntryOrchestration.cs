using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dynamitey.DynamicObjects;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using RouterLogIngestion.Functions.Entities;
using RouterLogIngestion.Models;

namespace RouterLogIngestion.Functions;

public class ProcessLogEntryOrchestration
{
    private readonly ILogger<ProcessLogEntryOrchestration> _logger;

    public ProcessLogEntryOrchestration(ILogger<ProcessLogEntryOrchestration> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [FunctionName(Constants.FunctionNames.ProcessLogEntryOrchestration)]
    public async Task<IpTablesLogEntry> RunOrchestration([OrchestrationTrigger] IDurableOrchestrationContext context)
    {
        var jsonString = context.GetInput<string>();
        var logEntry = IpTablesLogEntry.FromJsonString(jsonString);

        await context.CallEntityAsync(new EntityId(nameof(IpGeocodeQueue), Constants.Entities.EntityId), nameof(IpGeocodeQueue.AddLogEntryToQueue), logEntry);

        return await Task.FromResult(GetLogEntryFromString(jsonString));
    }

    private IpTablesLogEntry GetLogEntryFromString(string json)
    {
        var entry = IpTablesLogEntry.FromSyslogMessage(JObject.Parse(json));

        return entry;
    }
}

//public class GeocodeLogEntryBatchOrchestration
//{
//    private readonly ILogger<GeocodeLogEntryBatchOrchestration> _logger;

//    public GeocodeLogEntryBatchOrchestration(ILogger<GeocodeLogEntryBatchOrchestration> logger)
//    {
//        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//    }

//    public async Task RunOrchestration([OrchestrationTrigger] IDurableOrchestrationContext context)
//    {
//        var batch = context.GetInput<List<IpTablesLogEntry>>();

//    }
//}

//public class GeocodeLogEntryBatchActivity
//{
//    private readonly ILogger<GeocodeLogEntryBatchActivity> _logger;

//    public GeocodeLogEntryBatchActivity(ILogger<GeocodeLogEntryBatchActivity> logger)
//    {
//        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//    }
//}