﻿using System.Linq.Expressions;

namespace FreelanceBotBase.Infrastructure.Repository
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> GetAll();
        Task<TEntity?> GetByIdAsync(long id, CancellationToken cancellationToken);
        Task<TEntity?> GetByPredicateAsync(Expression<Func<TEntity, bool>> predicate);
        Task AddAsync(TEntity entity, CancellationToken cancellationToken);
        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken);
        Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken cancellationToken);
    }
}
