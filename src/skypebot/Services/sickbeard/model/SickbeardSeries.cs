using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace skypebot.Services.sickbeard.model
{
    public class SickbeardModel
    {
        public string message { get; set; }
        public string result { get; set; }
        public SickbeardData data { get; set; }
    }

    public class SickbeardData
    {
        public int langid { get; set; }
        public IEnumerable<SickbeardSeries> results { get; set; } 
    }
    public class SickbeardSeries
    {
        public DateTime? first_aired { get; set; }
        public string name { get; set; }
        public string tvdbid { get; set; }
    }

    public class SickbeardAddData
    {
        public string name { get; set; }
    }
    public class SickbeardAddModel
    {
        public string message { get; set; }
        public string result { get; set; }
        public SickbeardAddData data { get; set; }
    }
}
