using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RouterLogIngestion.Models;

public class IpTablesLogEntry
{
    [JsonProperty("WINDOW")]
    public string Window { get; set; }

    [JsonProperty("URGP")]
    public string Urgp { get; set; }

    [JsonProperty("TTL")]
    public string Ttl { get; set; }

    [JsonProperty("TOS")]
    public string Tos { get; set; }

    [JsonProperty("SRC")]
    public string Src { get; set; }

    [JsonProperty("SPT")]
    public string Spt { get; set; }

    [JsonProperty("SEQ")]
    public string Seq { get; set; }

    [JsonProperty("RES")]
    public string Res { get; set; }

    [JsonProperty("PROTO")]
    public string Proto { get; set; }

    [JsonProperty("PREC")]
    public string Prec { get; set; }

    [JsonProperty("OUT")]
    public string Out { get; set; }

    [JsonProperty("MARK")]
    public string Mark { get; set; }

    [JsonProperty("MAC")]
    public string Mac { get; set; }

    [JsonProperty("LEN")]
    public string Len { get; set; }

    [JsonProperty("IN")]
    public string In { get; set; }

    [JsonProperty("ID")]
    public string Id { get; set; }

    [JsonProperty("DST")]
    public string Dst { get; set; }

    [JsonProperty("DPT")]
    public string Dpt { get; set; }

    [JsonProperty("Ack")]
    public string Ack { get; set; }

    public static IpTablesLogEntry FromSyslogMessage(JObject syslogMessage)
    {
        var ipTablesEntry = syslogMessage["iptables"];
        var ipTablesString = ipTablesEntry.ToString();
        var parsedEntry = JsonConvert.DeserializeObject<IpTablesLogEntry>(ipTablesString);

        return parsedEntry;
    }
}
