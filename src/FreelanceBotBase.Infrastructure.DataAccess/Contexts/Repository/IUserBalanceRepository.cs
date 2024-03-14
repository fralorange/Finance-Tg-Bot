namespace FreelanceBotBase.Infrastructure.DataAccess.Contexts.Repository
{
    public interface IUserBalanceRepository
    {
        Task<IReadOnlyCollection<UserBalanceEntity>> GetAll();
        Task<UserBalanceEntity?> GetByUserIdAsync(long userId, CancellationToken cancellationToken);
        Task AddAsync(UserBalanceEntity entity, CancellationToken cancellationToken);
        Task UpdateAsync(UserBalanceEntity entity, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(UserBalanceEntity entity, CancellationToken cancellationToken);
    }
}
