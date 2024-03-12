using FreelanceBotBase.Bot.Commands.Base;
using FreelanceBotBase.Domain.FSM;
using FreelanceBotBase.Domain.States;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FreelanceBotBase.Bot.Commands.Enter
{
    public class EnterCallbackCommand : CallbackCommandBase
    {
        private readonly BotStateMachine _stateMachine;
        private readonly string _providerToken;

        public EnterCallbackCommand(ITelegramBotClient botClient, BotStateMachine stateMachine, string providerToken) : base(botClient)
        {
            _stateMachine = stateMachine;
            _providerToken = providerToken;
        }

        public async override Task<Message> ExecuteAsync(CallbackQuery data, CancellationToken cancellationToken)
        {
            _stateMachine.ChangeState(Trigger.AwaitUserInput);
            _stateMachine.SetWaitingForCommand(new ProcessEnteredCommand(BotClient, _providerToken));

            await BotClient.AnswerCallbackQueryAsync(
                callbackQueryId: data.Id,
                cancellationToken: cancellationToken);

            await BotClient.EditMessageTextAsync(
                chatId: data.Message!.Chat.Id,
                messageId: data.Message.MessageId,
                replyMarkup: null,
                text: data.Message.Text!,
                cancellationToken: cancellationToken);

            return await BotClient.SendTextMessageAsync(
                chatId: data.Message!.Chat.Id,
                text: "Введите сумму для пополнения",
                cancellationToken: cancellationToken);
        }
    }
}
