﻿using Dynamitey.DynamicObjects;

namespace RouterLogIngestion
{
    internal class Constants
    {
        internal class FunctionNames
        {
            internal const string ReceiveLogMessage = nameof(ReceiveLogMessage);

            internal const string ProcessLogEntryOrchestration = nameof(ProcessLogEntryOrchestration);
        }

        internal class Entities
        {
            internal const string EntityName = "IpGeocodeQueue";

            internal const string EntityId = "32EDBAE5-128A-43D6-822E-61D194C634FB";
        }
    }
}
