using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ruvents_api.Models
{
    public class RuventsContext : DbContext
    {
        public RuventsContext(DbContextOptions<RuventsContext> options) : base(options)
        {
        }

        public DbSet<Ruvent> Ruvents { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<RuventToUser> RuventToUser { get; set; }
    }
}
