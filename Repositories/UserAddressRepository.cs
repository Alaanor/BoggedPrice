using BoggedPrice.Models;
using BoggedPrice.Services;

namespace BoggedPrice.Repositories
{
    public class UserAddressRepository
    {
        private readonly DbService _db;

        public UserAddressRepository(DbService dbService)
        {
            _db = dbService;
        }

        public void Add(Wallet wallet)
        {
            _db.Wallets.Insert(wallet);
        }

        public Wallet? Get(ulong discordId)
        {
            return _db.Wallets.FindOne(x => x.DiscordId == discordId);
        }

        public void Update(Wallet wallet)
        {
            _db.Wallets.Update(wallet);
        }
    }
}