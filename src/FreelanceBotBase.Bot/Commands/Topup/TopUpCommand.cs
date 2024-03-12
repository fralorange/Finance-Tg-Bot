using FreelanceBotBase.Bot.Commands.Base;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace FreelanceBotBase.Bot.Commands.Topup
{
    public class TopUpCommand : TextCommandBase
    {
        public TopUpCommand(ITelegramBotClient botClient) : base(botClient) { }

        public async override Task<Message> ExecuteAsync(Message message, CancellationToken cancellationToken)
        {
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
                chatId: message.Chat.Id,
                text: "Выберите сумму для пополнения баланса:",
                replyMarkup: inlineKeyboard,
                cancellationToken: cancellationToken);
        }
    }
}
