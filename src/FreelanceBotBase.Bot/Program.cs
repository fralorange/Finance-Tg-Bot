﻿using FreelanceBotBase.Infrastructure.Configuration;
using FreelanceBotBase.Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Polling;
using FreelanceBotBase.Bot.Handlers.Update;
using FreelanceBotBase.Bot.Services.Receiver;
using FreelanceBotBase.Bot.Services.Polling;
using FreelanceBotBase.Bot.Commands.Factory;
using FreelanceBotBase.Bot.Services.ChatState;
using FreelanceBotBase.Infrastructure.Repository;
using FreelanceBotBase.Domain.UserBalance;
using FreelanceBotBase.Infrastructure.Repository.Database;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.Configure<BotConfiguration>(
            context.Configuration.GetSection(BotConfiguration.Configuration));

        services.Configure<ReceiverOptions>(
            context.Configuration.GetSection(nameof(ReceiverOptions)));


        services.AddHttpClient("telegram_bot_client")
            .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
            {
                BotConfiguration? botConfig = sp.GetConfiguration<BotConfiguration>();
                TelegramBotClientOptions options = new(botConfig.BotToken);
                return new TelegramBotClient(options, httpClient);
            });

        services.AddSingleton<IRepository<UserBalance>, InMemoryUserBalanceRepository>();
        services.AddSingleton<CommandFactory>();
        services.AddSingleton<ChatStateService>();
        services.AddScoped<UpdateHandler>();
        services.AddScoped<ReceiverService>();
        services.AddHostedService<PollingService>();
    })
    .Build();

await host.RunAsync();