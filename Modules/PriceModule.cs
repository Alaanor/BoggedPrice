using System.Threading.Tasks;
using BoggedPrice.Services;
using Discord.Commands;

namespace BoggedPrice.Modules
{
    public class PriceModule : ModuleBase<SocketCommandContext>
    {
        private readonly BoggedService _boggedService;

        public PriceModule(BoggedService boggedService)
        {
            _boggedService = boggedService;
        }

        [Command("price")]
        public async Task Price()
        {
            decimal? price = await _boggedService.GetUsdPrice();

            if (price != null)
            {
                await ReplyAsync($"> **1** BOG = **{price:N2}** USD");
                return;
            }

            await ReplyAsync("Failed to retrieve the price");
        }
    }
}