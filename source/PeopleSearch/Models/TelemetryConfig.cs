using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeopleSearch.Models
{
    public class TelemetryConfig
    {
        public bool Enabled { get; set; }
        public string Name { get; set; }
        public string ConnectionString { get; set; }
    }
}
