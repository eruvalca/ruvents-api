using ruvents_api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ruvents_api.ViewModels
{
    public class RuventDetailViewModel
    {
        public Ruvent Ruvent { get; set; }
        public UserViewModel User { get; set; }
    }
}
