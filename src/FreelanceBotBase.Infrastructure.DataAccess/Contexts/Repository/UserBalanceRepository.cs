
using FreelanceBotBase.Infrastructure.Repository;

namespace FreelanceBotBase.Infrastructure.DataAccess.Contexts.Repository
{
    public class UserBalanceRepository : IUserBalanceRepository
    {
        private readonly IRepository<UserBalanceEntity> _repository;

        public UserBalanceRepository(IRepository<UserBalanceEntity> repository)
            => _repository = repository;

        public Task<IReadOnlyCollection<UserBalanceEntity>> GetAll()
        {
            IReadOnlyCollection<UserBalanceEntity> collection = _repository.GetAll().ToList().AsReadOnly();
            return Task.FromResult(collection);
        }

        public Task<UserBalanceEntity?> GetByUserIdAsync(long userId, CancellationToken cancellationToken)
        {
            return _repository.GetByIdAsync(userId, cancellationToken);
        }

        public Task AddAsync(UserBalanceEntity entity, CancellationToken cancellationToken)
        {
            return _repository.ExecuteInTransactionAsync(() =>
            {
                return _repository.AddAsync(entity, cancellationToken);
            }, cancellationToken);
        }

        public Task UpdateAsync(UserBalanceEntity entity, CancellationToken cancellationToken)
        {
            return _repository.ExecuteInTransactionAsync(() =>
            {
                return _repository.UpdateAsync(entity, cancellationToken);
            }, cancellationToken);
        }

        public Task<bool> DeleteAsync(UserBalanceEntity entity, CancellationToken cancellationToken)
        {
            return _repository.DeleteAsync(entity, cancellationToken);
        }
    }
}
