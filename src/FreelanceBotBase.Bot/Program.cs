using FreelanceBotBase.Bot;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddConfigurations(context);

        services.AddHttpClients();

        services.AddRepositories();
        services.AddFactories();
        services.AddHandlers();
        services.AddServices();

        services.AddDbContext();
        services.AddMapper();
    })
    .ConfigureAppConfiguration((_, config) =>
    {
        config.AddUserSecrets<Program>();
    })
    .Build();

await host.RunAsync();