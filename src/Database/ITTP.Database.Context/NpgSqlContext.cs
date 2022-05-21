using ITTP.Database.Context.Configuration;
using ITTP.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace ITTP.Database.Context
{
    public class NpgSqlContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public NpgSqlContext(DbContextOptions<NpgSqlContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
        }
    }
}
