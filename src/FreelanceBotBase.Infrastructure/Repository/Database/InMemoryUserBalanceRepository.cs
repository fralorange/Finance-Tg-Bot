using FreelanceBotBase.Domain.UserBalance;
using System.Linq.Expressions;

namespace FreelanceBotBase.Infrastructure.Repository.Database
{
    public class InMemoryUserBalanceRepository : IRepository<UserBalance>
    {
        private readonly List<UserBalance> _userBalances;

        public InMemoryUserBalanceRepository()
        {
            _userBalances = [];
        }

        public IQueryable<UserBalance> GetAll()
        {
            return _userBalances.AsQueryable();
        }

        public Task<UserBalance?> GetByPredicateAsync(Expression<Func<UserBalance, bool>> predicate)
        {
            return Task.FromResult(_userBalances.FirstOrDefault(predicate.Compile()));
        }

        public Task AddAsync(UserBalance entity, CancellationToken cancellationToken)
        {
            _userBalances.Add(entity);
            return Task.CompletedTask;
        }

        public Task<bool> DeleteAsync(UserBalance entity, CancellationToken cancellationToken)
        {
            return Task.FromResult(_userBalances.Remove(entity));
        }

        public Task UpdateAsync(UserBalance entity, CancellationToken cancellationToken)
        {
            var userBalance = _userBalances.Find(ub => ub.UserId == entity.UserId);
            if (userBalance is not null)
                userBalance.Balance = entity.Balance;
            return Task.CompletedTask;
        }
    }
}
