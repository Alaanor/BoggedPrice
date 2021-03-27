using System;
using BoggedPrice.Models;
using LiteDB;

namespace BoggedPrice.Services
{
    public class DbService
    {
        public readonly ILiteCollection<Wallet> Wallets;

        public DbService()
        {
            string dbPath = Environment.GetEnvironmentVariable("DB_PATH") ?? "/app/data/db";
            LiteDatabase db = new(dbPath);
            Wallets = db.GetCollection<Wallet>();
            Wallets.EnsureIndex(x => x.DiscordId);
        }
    }
}