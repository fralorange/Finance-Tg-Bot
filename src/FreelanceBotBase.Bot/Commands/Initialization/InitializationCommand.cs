using FreelanceBotBase.Bot.Commands.Base;
using FreelanceBotBase.Bot.Commands.Null;
using FreelanceBotBase.Domain.UserBalance;
using FreelanceBotBase.Infrastructure.DataAccess.Contexts.Repository;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FreelanceBotBase.Bot.Commands.Initialization
{
    public class InitializationCommand : TextCommandBase
    {
        private readonly IUserBalanceRepository _repository;

        public InitializationCommand(ITelegramBotClient botClient, IUserBalanceRepository repository) : base(botClient)
            => _repository = repository;

        public async override Task<Message> ExecuteAsync(Message message, CancellationToken cancellationToken)
        {
            var chatId = message.Chat.Id;
            var userId = message.From!.Id;
            var userBalance = await _repository.GetByUserIdAsync(userId, cancellationToken);
            if (userBalance is null)
            {
                try
                {
                    await _repository.AddAsync(new UserBalance { UserId = userId, Balance = 0 }, cancellationToken);

                    return await BotClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: $"Добро пожаловать {message.From.Username}\nВаш стартовый баланс: 0",
                        cancellationToken: cancellationToken);
                }
                catch (Exception)
                {
                    return await BotClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: $"ОШИБКА: Произошла ошибка при инициализации бота.\nПожалуйста, попробуйте позже или свяжитесь с администратором бота.",
                        cancellationToken: cancellationToken);
                }
                
            }

            return await new NullCommand().ExecuteAsync(message, cancellationToken);
        }
    }
}
