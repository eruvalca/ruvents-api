using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ruvents_api.Models
{
    public class RuventToUser
    {
        public int RuventToUserId { get; set; }
        public bool IsAttending { get; set; }

        public int RuventId { get; set; }
        public Ruvent Ruvent { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
