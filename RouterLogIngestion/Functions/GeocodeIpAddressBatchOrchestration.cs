using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using RouterLogIngestion.Models;

namespace RouterLogIngestion.Functions;

public class GeocodeIpAddressBatchOrchestration
{
    private readonly ILogger<GeocodeIpAddressBatchOrchestration> _logger;

    public GeocodeIpAddressBatchOrchestration(ILogger<GeocodeIpAddressBatchOrchestration> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [FunctionName(nameof(GeocodeIpAddressBatchOrchestration))]
    public async Task RunOrchestration([OrchestrationTrigger] IDurableOrchestrationContext context)
    {
        var input = context.GetInput<List<IpTablesLogEntry>>();
        await context.CallActivityAsync(nameof(GeocodeIpAddressBatchActivity), input);
    }
}