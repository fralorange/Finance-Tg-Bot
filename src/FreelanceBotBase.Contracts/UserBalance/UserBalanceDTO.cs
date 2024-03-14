namespace FreelanceBotBase.Contracts.UserBalance
{
    public class UserBalanceDTO
    {
        public required long UserId { get; set; }
        public required decimal Balance { get; set; }
    }
}
