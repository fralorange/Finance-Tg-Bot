using FreelanceBotBase.Infrastructure.DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FreelanceBotBase.Infrastructure.DataAccess
{
    public class DbInitializer : IDbInitializer
    {
        private readonly DbContext _context;

        public DbInitializer(DbContext context)
            => _context = context;

        public void Initialize()
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }
    }
}
