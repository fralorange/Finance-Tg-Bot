using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FreelanceBotBase.Infrastructure.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected DbContext DbContext { get; }
        protected DbSet<TEntity> DbSet { get; }

        public Repository(DbContext dbContext)
        {
            DbContext = dbContext;
            DbSet = DbContext.Set<TEntity>();
        }

        public IQueryable<TEntity> GetAll()
        {
            return DbSet;
        }

        public async Task<TEntity?> GetByIdAsync(long id, CancellationToken cancellationToken)
        {
            return await DbSet.FindAsync([id], cancellationToken: cancellationToken);   
        }

        public async Task<TEntity?> GetByPredicateAsync(Expression<Func<TEntity, bool>> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            return await DbSet.AsNoTracking().FirstOrDefaultAsync(predicate);
        }

        public async Task AddAsync(TEntity entity, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(entity);

            await DbSet.AddAsync(entity, cancellationToken);
            await DbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(entity);

            DbSet.Remove(entity);
            int affectedRows = await DbContext.SaveChangesAsync(cancellationToken);
            return affectedRows > 0;
        }

        public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(entity);

            DbSet.Update(entity);
            await DbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken cancellationToken)
        {
            using var transaction = await DbContext.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                await action();

                await DbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
