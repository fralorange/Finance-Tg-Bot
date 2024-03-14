using FreelanceBotBase.Bot.Commands.Base;
using FreelanceBotBase.Infrastructure.DataAccess.Contexts.Repository;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FreelanceBotBase.Bot.Commands.Topup
{
    public class SuccessfulPaymentCommand : TextCommandBase
    {
        private readonly IUserBalanceRepository _repository;

        public SuccessfulPaymentCommand(ITelegramBotClient botClient, IUserBalanceRepository repository) : base(botClient)
            => _repository = repository;

        public override async Task<Message> ExecuteAsync(Message message, CancellationToken cancellationToken)
        {
            var chatId = message.Chat.Id;

            var amount = message.SuccessfulPayment!.TotalAmount / 100;
            var userId = message.From!.Id;
            try
            {
                var userBalance = await _repository.GetByUserIdAsync(userId, cancellationToken);
                userBalance!.Balance += amount;

                await _repository.UpdateAsync(userBalance, cancellationToken);

                return await BotClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: $"Баланс пользователя `{message.From.Username}` успешно пополнен на {amount} руб.\nТекущий баланс составляет: {userBalance.Balance} руб.",
                    cancellationToken: cancellationToken);
            }
            catch (Exception)
            {
                return await BotClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: $"ОШИБКА: Произошла ошибка при пополнении баланса.\nПожалуйста, свяжитесь с администратором бота.",
                    cancellationToken: cancellationToken);
            }
        }
    }
}
