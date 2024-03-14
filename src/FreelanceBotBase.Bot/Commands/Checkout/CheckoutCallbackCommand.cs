using FreelanceBotBase.Bot.Commands.Base;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Payments;

namespace FreelanceBotBase.Bot.Commands.Checkout
{
    public class CheckoutCallbackCommand : CallbackCommandBase
    {
        private readonly string _providerToken;
        private readonly int _amount;

        public CheckoutCallbackCommand(ITelegramBotClient botClient, string providerToken, int amount) : base(botClient)
        {
            _providerToken = providerToken;
            _amount = amount;
        }

        public override async Task<Message> ExecuteAsync(CallbackQuery data, CancellationToken cancellationToken)
        {
            if (data.Message!.From!.IsBot)
            {
                await BotClient.AnswerCallbackQueryAsync(
                    callbackQueryId: data.Id,
                    text: $"Вы выбрали пополнение баланса на {_amount} руб.",
                    cancellationToken: cancellationToken);

                await BotClient.EditMessageTextAsync(
                    chatId: data.Message!.Chat.Id,
                    messageId: data.Message.MessageId,
                    replyMarkup: null,
                    text: data.Message.Text!,
                    cancellationToken: cancellationToken);
            }

            var prices = new List<LabeledPrice>
            {
                new(
                    label: $"Пополнение баланса на {_amount} рублей",
                    amount: _amount * 100),
            };

            return await BotClient.SendInvoiceAsync(
                chatId: data.Message!.Chat.Id,
                title: "Пополнение баланса",
                description: $"Пополнение баланса на счёт пользователя: {data.Message.From!.Username}",
                currency: "RUB",
                isFlexible: false,
                prices: prices,
                payload: "invoice-payload",
                startParameter: "top-up-balance",
                providerToken: _providerToken,
                cancellationToken: cancellationToken);
        }
    }
}
