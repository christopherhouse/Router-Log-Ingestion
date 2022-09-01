using System;
using Newtonsoft.Json;

namespace RouterLogIngestion.Models
{
    public class GeolocationQuery
    {
        public GeolocationQuery(string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                throw new ArgumentException(nameof(ipAddress));
            }

            Query = ipAddress;
        }

        [JsonProperty("query")]
        public string Query { get; }
    }
}
