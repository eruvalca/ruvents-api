using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ruvents_api.Models
{
    public class Ruvent
    {
        public int RuventId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public DateTime Date { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
