using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RouterLogIngestion.Models;

namespace RouterLogIngestion.Functions
{
    public class ReceiveLogMessage
    {
        private readonly ILogger<ReceiveLogMessage> _logger;

        public ReceiveLogMessage(ILogger<ReceiveLogMessage> log)
        {
            _logger = log;
        }

        [FunctionName("ReceiveLogMessage")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [DurableClient] IDurableOrchestrationClient orchestrationClient)
        {
            using var reader = new StreamReader(req.Body);
            var bodyString = await reader.ReadToEndAsync();
            var json = JObject.Parse(bodyString);
            const string fwObjectName = "iptables";

            if (json.ContainsKey(fwObjectName))
            {
                var ipTablesEntry = IpTablesLogEntry.FromSyslogMessage(json);
                Console.WriteLine(ipTablesEntry.Dst);
            }

            var orchId  = await orchestrationClient.StartNewAsync("");
            var result = orchestrationClient.CreateCheckStatusResponse(req, orchId);

            return result;
        }
    }
}

