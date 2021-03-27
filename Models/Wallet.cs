#nullable disable

namespace BoggedPrice.Models
{
    public class Wallet
    {
        public int Id { get; set; }
        public ulong DiscordId { get; set; }
        public string Address { get; set; }
    }
}