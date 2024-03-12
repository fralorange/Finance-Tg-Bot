using FreelanceBotBase.Bot.Commands.Base;
using FreelanceBotBase.Domain.UserBalance;
using FreelanceBotBase.Infrastructure.Repository;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace FreelanceBotBase.Bot.Commands.Balance
{
    public class BalanceCommand : TextCommandBase
    {
        private readonly IRepository<UserBalance> _repository;

        public BalanceCommand(ITelegramBotClient botClient, IRepository<UserBalance> repository) : base(botClient)
            => _repository = repository;

        public override async Task<Message> ExecuteAsync(Message message, CancellationToken cancellationToken)
        {
            var userId = message.From!.Id;
            var userBalance = await _repository.GetByPredicateAsync(ub => ub.UserId == userId);

            var chatId = message.Chat.Id;

            if (userBalance is null)
            {
                return await BotClient.SendTextMessageAsync(
                    chatId: chatId,
                    parseMode: ParseMode.Html,
                    text: "ОШИБКА: Пользователь не зарегистрирован в системе!\nПодсказка: введите <code>/start</code>",
                    cancellationToken: cancellationToken);
            }

            return await BotClient.SendTextMessageAsync(
                chatId: chatId,
                text: $"Баланс пользователя {message.From.Username} составляет {userBalance.Balance} руб.",
                cancellationToken: cancellationToken);
        }
    }
}
