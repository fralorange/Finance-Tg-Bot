using FreelanceBotBase.Bot.Commands.Checkout;
using FreelanceBotBase.Bot.Commands.Enter;
using FreelanceBotBase.Bot.Commands.Interface;
using FreelanceBotBase.Bot.Commands.Null;
using FreelanceBotBase.Bot.Commands.Topup;
using FreelanceBotBase.Bot.Services.ChatState;
using FreelanceBotBase.Domain.FSM;
using FreelanceBotBase.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace FreelanceBotBase.Bot.Commands.Factory
{
    public class CommandFactory
    {
        private readonly ITelegramBotClient _botClient;
        private readonly string _providerToken;

        public CommandFactory(ITelegramBotClient botClient, IOptions<BotConfiguration> config)
        {
            _botClient = botClient;
            _providerToken = config.Value.ProviderToken;
        }

        public ITextCommand CreateCommand(string commandName)
        {
            return commandName switch
            {
                "/topup" => new TopUpCommand(_botClient),
                _ => new NullCommand()
            };
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
