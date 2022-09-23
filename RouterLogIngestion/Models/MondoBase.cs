using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouterLogIngestion.Models
{
    public class MondoBase
    {
        public MondoBase(GeocodeResult geocodeData, IpTablesLogEntry logEntry)
        {
            GeocodeData = geocodeData;
            LogEntry = logEntry;
        }

        public GeocodeResult GeocodeData { get; set; }

        public IpTablesLogEntry LogEntry { get; set; }
    }
}
