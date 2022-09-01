using Newtonsoft.Json;

namespace RouterLogIngestion.Models;

public class GeocodeResult
{
    [JsonProperty("as")]
    public string AutonomousSystem { get; set; }

    [JsonProperty("city")]
    public string City { get; set; }

    [JsonProperty("country")]
    public string Country { get; set; }

    [JsonProperty("countryCode")]
    public string CountryCode { get; set; }

    [JsonProperty("isp")]
    public string InternetServiceProvider { get; set; }

    [JsonProperty("lat")]
    public decimal Latitude { get; set; }

    [JsonProperty("lon")]
    public decimal Longitude { get; set; }

    [JsonProperty("org")]
    public string Organization { get; set; }

    [JsonProperty("query")]
    public string SourceIpAddress { get; set; }

    [JsonProperty("region")]
    public string Region { get; set; }

    [JsonProperty("regionName")]
    public string RegionName { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("timezone")]
    public string Timezone { get; set; }

    [JsonProperty("zip")]
    public string ZipCode { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; }
}