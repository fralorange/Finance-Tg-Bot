using FreelanceBotBase.Bot.Commands.Base;
using FreelanceBotBase.Domain.UserBalance;
using FreelanceBotBase.Infrastructure.Repository;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FreelanceBotBase.Bot.Commands.Topup
{
    public class SuccessfulPaymentCommand : TextCommandBase
    {
        private readonly IRepository<UserBalance> _repository;

        public SuccessfulPaymentCommand(ITelegramBotClient botClient, IRepository<UserBalance> repository) : base(botClient)
            => _repository = repository;

        public override async Task<Message> ExecuteAsync(Message message, CancellationToken cancellationToken)
        {
            var chatId = message.Chat.Id;

            var amount = message.SuccessfulPayment!.TotalAmount / 100;
            var userId = message.From!.Id;

            var userBalance = await _repository.GetByPredicateAsync(ub => ub.UserId == userId);
            userBalance!.Balance += amount;

            await _repository.UpdateAsync(userBalance, cancellationToken);

            return await BotClient.SendTextMessageAsync(
                chatId: chatId,
                text: $"Баланс пользователя `{message.From.Username}` успешно пополнен на {amount} руб.\nТекущий баланс составляет: {userBalance.Balance} руб.",
                cancellationToken: cancellationToken);
        }
    }
}
