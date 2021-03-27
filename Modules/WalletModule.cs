using System.Threading.Tasks;
using BoggedPrice.Models;
using BoggedPrice.Repositories;
using BoggedPrice.Services;
using Discord.Commands;
using Discord.WebSocket;

namespace BoggedPrice.Modules
{
    [Group("wallet")]
    public class WalletModule : ModuleBase<SocketCommandContext>
    {
        private readonly UserAddressRepository _repository;
        private readonly BoggedService _boggedService;

        public WalletModule(UserAddressRepository repository, BoggedService boggedService)
        {
            _repository = repository;
            _boggedService = boggedService;
        }


        [Command("set")]
        public Task SetWallet(string address)
        {
            ulong discordId = Context.User.Id;
            Wallet? user = _repository.Get(discordId);

            if (user is { } existingUser)
            {
                existingUser.Address = address;
                _repository.Update(existingUser);

                return ReplyAsync("Updated");
            }

            _repository.Add(new Wallet
            {
                Address = address,
                DiscordId = discordId
            });

            return ReplyAsync("Added");
        }

        [Command("get")]
        public Task GetWallet(SocketUser? target = null)
        {
            bool isTargetingHimself = target == null;
            Wallet? wallet = _repository.Get(target is { } user ? user.Id : Context.User.Id);

            if (wallet is { } existingWallet)
            {
                return ReplyAsync(isTargetingHimself
                    ? $"Your wallet address is `{existingWallet.Address}`"
                    : $"His wallet address is `{existingWallet.Address}`");
            }

            return ReplyAsync("This user does not have any wallet registered there.");
        }

        [Command("value")]
        public async Task GetValue(SocketUser? target = null)
        {
            bool isTargetingHimself = target == null;
            Wallet? wallet = _repository.Get(target is { } user ? user.Id : Context.User.Id);

            if (wallet == null)
            {
                await ReplyAsync("This user does not have any wallet registered there.");
                return;
            }

            decimal amount = await _boggedService.GetBalance(wallet.Address);
            decimal price = await _boggedService.GetUsdPrice();
            decimal ownInUsd = amount * price;

            string pronoun = isTargetingHimself ? "You" : "They";

            await ReplyAsync($"{pronoun} own **{amount:N2}** BOG. (@{price:N2}$, {ownInUsd:N2}$)");
        }
    }
}