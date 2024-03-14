using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FreelanceBotBase.Infrastructure.DataAccess.Contexts.Configuration
{
    public class UserBalanceConfiguration : IEntityTypeConfiguration<UserBalanceEntity>
    {
        public void Configure(EntityTypeBuilder<UserBalanceEntity> builder)
        {
            builder.ToTable(nameof(Domain.UserBalance.UserBalance));

            builder.HasKey(ub => ub.UserId);

            builder.Property(ub => ub.UserId).IsRequired().ValueGeneratedNever();
            builder.Property(ub => ub.Balance).IsRequired().ValueGeneratedNever();
        }
    }
}
