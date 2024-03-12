using FreelanceBotBase.Bot.Commands.Interface;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FreelanceBotBase.Bot.Commands.Base
{
    public abstract class TextCommandBase : ITextCommand
    {
        protected readonly ITelegramBotClient BotClient;
        public TextCommandBase(ITelegramBotClient botClient) => BotClient = botClient;

        public abstract Task<Message> ExecuteAsync(Message message, CancellationToken cancellationToken);
    }
}
