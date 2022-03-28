#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YABC.Models;

namespace YABC.Data
{
    public class YABCContext : DbContext
    {
        public YABCContext (DbContextOptions<YABCContext> options)
            : base(options)
        {
        }

        public DbSet<YABC.Models.Block> Block { get; set; }

        public DbSet<YABC.Models.Person> Person { get; set; }
    }
}
