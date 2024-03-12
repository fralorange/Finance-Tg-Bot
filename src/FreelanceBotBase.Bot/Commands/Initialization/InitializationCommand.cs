using FreelanceBotBase.Bot.Commands.Base;
using FreelanceBotBase.Bot.Commands.Null;
using FreelanceBotBase.Domain.UserBalance;
using FreelanceBotBase.Infrastructure.Repository;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FreelanceBotBase.Bot.Commands.Initialization
{
    public class InitializationCommand : TextCommandBase
    {
        private readonly IRepository<UserBalance> _repository;

        public InitializationCommand(ITelegramBotClient botClient, IRepository<UserBalance> repository) : base(botClient)
            => _repository = repository;

        public async override Task<Message> ExecuteAsync(Message message, CancellationToken cancellationToken)
        {
            var userId = message.From!.Id;
            var userBalance = await _repository.GetByPredicateAsync(ub => ub.UserId == userId);
            if (userBalance is null)
            {
                await _repository.AddAsync(new UserBalance { UserId = userId, Balance = 0 }, cancellationToken);
                
                return await BotClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: $"Добро пожаловать {message.From.Username}\nВаш стартовый баланс: 0",
                    cancellationToken: cancellationToken);
            }

            return await new NullCommand().ExecuteAsync(message, cancellationToken);
        }
    }
}
