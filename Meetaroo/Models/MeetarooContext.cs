using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Meetaroo.Models
{
    public class MeetarooContext : DbContext
    {
        public DbSet<Organization> Organizations { get; set; }

        public MeetarooContext(DbContextOptions<MeetarooContext> options) : base(options)
        {

        }
    }
}
