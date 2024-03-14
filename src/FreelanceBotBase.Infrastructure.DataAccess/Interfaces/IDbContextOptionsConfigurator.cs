using Microsoft.EntityFrameworkCore;

namespace FreelanceBotBase.Infrastructure.DataAccess.Interfaces
{
    public interface IDbContextOptionsConfigurator<TContext> where TContext : DbContext
    {
        void Configure(DbContextOptionsBuilder<TContext> options);
    }
}
