using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PeopleSearch.Data.Entities;

namespace PeopleSearch.Models
{
    public class Stats
    {
        public int TotalPeople { get; set; }
        public IEnumerable<NameStat> NameStats { get; set; }
    }
}
