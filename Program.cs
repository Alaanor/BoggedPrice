using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BoggedPrice.Repositories;
using BoggedPrice.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace BoggedPrice
{
    internal class Program
    {
        private static void Main()
            => MainAsync().GetAwaiter().GetResult();

        private static async Task MainAsync()
        {
            await using ServiceProvider services = ConfigureServices();
            var client = services.GetRequiredService<DiscordSocketClient>();

            client.Ready += () => OnReady(services);
            client.Log += LogAsync;
            services.GetRequiredService<CommandService>().Log += LogAsync;

            await client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("DISCORD_BOT_TOKEN"));
            await client.StartAsync();


            await Task.Delay(Timeout.Infinite);
        }

        private static async Task OnReady(IServiceProvider services)
        {
            await services.GetRequiredService<CommandHandlingService>().InitializeAsync();
            await services.GetRequiredService<BoggedService>().InitializeAsync();
            await services.GetRequiredService<PresencePriceService>().InitializeAsync();
        }

        private static Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        private static ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<BoggedService>()
                .AddSingleton<HttpClient>()
                .AddSingleton<DbService>()
                .AddSingleton<UserAddressRepository>()
                .AddSingleton<PresencePriceService>()
                .BuildServiceProvider();
        }
    }
}