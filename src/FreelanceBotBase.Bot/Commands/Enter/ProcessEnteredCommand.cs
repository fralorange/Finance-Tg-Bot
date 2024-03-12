using FreelanceBotBase.Bot.Commands.Base;
using FreelanceBotBase.Bot.Commands.Checkout;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FreelanceBotBase.Bot.Commands.Enter
{
    public class ProcessEnteredCommand : TextCommandBase
    {
        private readonly string _providerToken;

        public ProcessEnteredCommand(ITelegramBotClient botClient, string providerToken) : base(botClient)
            => _providerToken = providerToken;

        public override async Task<Message> ExecuteAsync(Message message, CancellationToken cancellationToken)
        {
            if (int.TryParse(message.Text, out var amount) && !(amount > 1000000))
            {
                var data = new CallbackQuery
                {
                    Message = message
                };
                // костыль =)
                return await new CheckoutCallbackCommand(BotClient, _providerToken, amount).ExecuteAsync(data, cancellationToken);
            }
            else
            {
                return await BotClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "ОШИБКА: Требуется ввести ЧИСЛО (до 1 000 000).",
                    cancellationToken: cancellationToken);
            }
        }
    }
}
