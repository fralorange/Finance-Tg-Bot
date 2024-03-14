using FreelanceBotBase.Bot.Commands.Base;
using FreelanceBotBase.Infrastructure.DataAccess.Contexts.Repository;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace FreelanceBotBase.Bot.Commands.Topup
{
    public class TopUpCommand : TextCommandBase
    {
        private readonly IUserBalanceRepository _repository;

        public TopUpCommand(ITelegramBotClient botClient, IUserBalanceRepository repository) : base(botClient)
            => _repository = repository;

        public async override Task<Message> ExecuteAsync(Message message, CancellationToken cancellationToken)
        {
            var userId = message.From!.Id;
            var userBalance = await _repository.GetByUserIdAsync(userId, cancellationToken);

            var chatId = message.Chat.Id;

            if (userBalance is null)
            {
                return await BotClient.SendTextMessageAsync(
                    chatId: chatId,
                    parseMode: ParseMode.Html,
                    text: "ОШИБКА: Пользователь не зарегистрирован в системе!\nПодсказка: введите <code>/start</code>",
                    cancellationToken: cancellationToken);
            }

            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("100 руб.", "checkout_100"),
                    InlineKeyboardButton.WithCallbackData("200 руб.", "checkout_200"),
                    InlineKeyboardButton.WithCallbackData("500 руб.", "checkout_500"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Другая сумма", "checkout_other"),
                }
            });

            return await BotClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Выберите сумму для пополнения баланса:",
                replyMarkup: inlineKeyboard,
                cancellationToken: cancellationToken);
        }
    }
}
