using FreelanceBotBase.Infrastructure.DataAccess.Contexts.Configuration;
using Microsoft.EntityFrameworkCore;

namespace FreelanceBotBase.Infrastructure.DataAccess
{
    public class BaseDbContext : DbContext
    {
        public BaseDbContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserBalanceConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
