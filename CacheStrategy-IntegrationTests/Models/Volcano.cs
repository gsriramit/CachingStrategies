using System;
using System.Collections.Generic;
using System.Text;

namespace CacheStrategy_IntegrationTests.Models
{
    public class Volcano
    {
        public string VolcanoName { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public Location Location { get; set; }
        public int Elevation { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string LastKnownEruption { get; set; }
        public string id { get; set; }
    }

    public class Location
    {
        public string type { get; set; }
        public float[] coordinates { get; set; }
    }
}
