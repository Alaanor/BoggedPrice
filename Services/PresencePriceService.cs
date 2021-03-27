using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace BoggedPrice.Services
{
    public class PresencePriceService
    {
        private readonly DiscordSocketClient _client;
        private readonly BoggedService _boggedService;

        public PresencePriceService(BoggedService boggedService, DiscordSocketClient client)
        {
            _client = client;
            _client.SetActivityAsync(new Game("Bog price", ActivityType.Watching));
            _boggedService = boggedService;
        }

        public Task InitializeAsync()
        {
            int refreshRate = GetRefreshRate();
            var task = new PeriodicTask();

            task.Run(async () =>
            {
                decimal price = await _boggedService.GetUsdPrice();

                foreach (var guild in _client.Guilds)
                {
                    await guild
                        .GetUser(_client.CurrentUser.Id)
                        .ModifyAsync(x => { x.Nickname = $"BOG: {price:N2}$"; });

                    await Task.Delay(1000);
                }
            }, TimeSpan.FromSeconds(refreshRate));

            return Task.CompletedTask;
        }

        private static int GetRefreshRate()
        {
            int refreshRate = 60;

            try
            {
                refreshRate = int.Parse(
                    Environment.GetEnvironmentVariable("REFRESH_RATE") ??
                    throw new InvalidOperationException()
                );
            }
            catch (Exception)
            {
                // ignored
            }

            return refreshRate;
        }
    }

    public class PeriodicTask
    {
        private bool _firstRun = true;

        public async Task Run(Action action, TimeSpan period, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (!_firstRun)
                {
                    await Task.Delay(period, cancellationToken);
                }

                if (cancellationToken.IsCancellationRequested)
                    continue;

                action();
                _firstRun = false;
            }
        }

        public Task Run(Action action, TimeSpan period)
        {
            return Run(action, period, CancellationToken.None);
        }
    }
}