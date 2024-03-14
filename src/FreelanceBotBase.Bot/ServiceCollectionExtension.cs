using FreelanceBotBase.Bot.Commands.Factory;
using FreelanceBotBase.Bot.Handlers.Update;
using FreelanceBotBase.Bot.Services.ChatState;
using FreelanceBotBase.Bot.Services.Polling;
using FreelanceBotBase.Bot.Services.Receiver;
using FreelanceBotBase.Infrastructure.Configuration;
using FreelanceBotBase.Infrastructure.DataAccess.Contexts.Repository;
using FreelanceBotBase.Infrastructure.Repository;
using FreelanceBotBase.Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Microsoft.Extensions.Hosting;
using Telegram.Bot.Polling;
using FreelanceBotBase.Infrastructure.DataAccess.Interfaces;
using FreelanceBotBase.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using FreelanceBotBase.Infrastructure.ComponentRegistrar.Mappers.UserBalance;

namespace FreelanceBotBase.Bot
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddConfigurations(this IServiceCollection services, HostBuilderContext context)
        {
            services.Configure<BotConfiguration>(
                context.Configuration.GetSection(BotConfiguration.Configuration));

            services.Configure<ReceiverOptions>(
                context.Configuration.GetSection(nameof(ReceiverOptions)));

            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddSingleton<ChatStateService>();
            services.AddScoped<ReceiverService>();
            services.AddHostedService<PollingService>();

            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUserBalanceRepository, UserBalanceRepository>();

            return services;
        }

        public static IServiceCollection AddFactories(this IServiceCollection services)
        {
            services.AddScoped<CommandFactory>();

            return services;
        }

        public static IServiceCollection AddHandlers(this IServiceCollection services)
        {
            services.AddScoped<UpdateHandler>();

            return services;
        }

        public static IServiceCollection AddHttpClients(this IServiceCollection services)
        {
            services.AddHttpClient("telegram_bot_client")
                .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
                {
                    BotConfiguration? botConfig = sp.GetConfiguration<BotConfiguration>();
                    TelegramBotClientOptions options = new(botConfig.BotToken);
                    return new TelegramBotClient(options, httpClient);
                });

            return services;
        }

        public static IServiceCollection AddDbContext(this IServiceCollection services)
        {
#if DEBUG
            services.AddScoped<IDbInitializer, DbInitializer>();
#endif
            services.AddSingleton<IDbContextOptionsConfigurator<BaseDbContext>, BaseDbContextOptionsConfigurator>();
            services.AddDbContext<BaseDbContext>((sp, opt) =>
            {
                var configurator = sp.GetRequiredService<IDbContextOptionsConfigurator<BaseDbContext>>();
                configurator.Configure((DbContextOptionsBuilder<BaseDbContext>)opt);
            });
            services.AddScoped<DbContext, BaseDbContext>();

            return services;
        }

        public static IServiceCollection AddMapper(this IServiceCollection services)
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<UserBalanceMapper>());
            config.AssertConfigurationIsValid();

            IMapper mapper = config.CreateMapper();
            services.AddSingleton(mapper);

            return services;
        }
    }
}
