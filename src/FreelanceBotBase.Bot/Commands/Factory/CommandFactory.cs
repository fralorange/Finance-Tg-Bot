using FreelanceBotBase.Bot.Commands.Balance;
using FreelanceBotBase.Bot.Commands.Checkout;
using FreelanceBotBase.Bot.Commands.Enter;
using FreelanceBotBase.Bot.Commands.Initialization;
using FreelanceBotBase.Bot.Commands.Interface;
using FreelanceBotBase.Bot.Commands.Null;
using FreelanceBotBase.Bot.Commands.Topup;
using FreelanceBotBase.Domain.FSM;
using FreelanceBotBase.Domain.UserBalance;
using FreelanceBotBase.Infrastructure.Configuration;
using FreelanceBotBase.Infrastructure.Repository;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace FreelanceBotBase.Bot.Commands.Factory
{
    public class CommandFactory
    {
        private readonly ITelegramBotClient _botClient;
        private readonly string _providerToken;
        private readonly IRepository<UserBalance> _repository;

        public CommandFactory(ITelegramBotClient botClient, IOptions<BotConfiguration> config, IRepository<UserBalance> repository)
        {
            _botClient = botClient;
            _providerToken = config.Value.ProviderToken;
            _repository = repository;
        }

        public ITextCommand CreateCommand(string commandName)
        {
            return commandName switch
            {
                "/start" => new InitializationCommand(_botClient, _repository),
                "/topup" => new TopUpCommand(_botClient, _repository),
                "/balance" => new BalanceCommand(_botClient, _repository),
                _ => new NullCommand()
            };
        }

        public ITextCommand CreateSuccessfulPaymentCommand()
        {
            return new SuccessfulPaymentCommand(_botClient, _repository);
        }

        public ICallbackCommand CreateCallbackCommand(string commandParam, BotStateMachine stateMachine)
        {
            return commandParam switch
            {
                "checkout_100" => new CheckoutCallbackCommand(_botClient, _providerToken, 100),
                "checkout_200" => new CheckoutCallbackCommand(_botClient, _providerToken, 200),
                "checkout_500" => new CheckoutCallbackCommand(_botClient, _providerToken, 500),
                "checkout_other" => new EnterCallbackCommand(_botClient, stateMachine, _providerToken),
                _ => new NullCommand(),
            };
        }
    }
}
