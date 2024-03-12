using FreelanceBotBase.Bot.Commands.Interface;
using Telegram.Bot.Types;

namespace FreelanceBotBase.Bot.Commands.Null
{
    public class NullCommand : ICallbackCommand, ITextCommand
    {
        public Task<Message> ExecuteAsync(Message data, CancellationToken cancellationToken)
        {
            return Task.FromResult(new Message());
        }

        public Task<Message> ExecuteAsync(CallbackQuery data, CancellationToken cancellationToken)
        {
            return Task.FromResult(new Message());
        }
    }
}
