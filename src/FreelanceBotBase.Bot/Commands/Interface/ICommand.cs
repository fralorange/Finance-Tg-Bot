using Telegram.Bot.Types;

namespace FreelanceBotBase.Bot.Commands.Interface
{
    public interface ICommand<T>
    {
        Task<Message> ExecuteAsync(T data, CancellationToken cancellationToken);
    }
}
