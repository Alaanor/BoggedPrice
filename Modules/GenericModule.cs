using System.Threading.Tasks;
using Discord.Commands;

namespace BoggedPrice.Modules
{
    public class GenericModule : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        [Alias("pong", "hello")]
        public Task PingAsync()
            => ReplyAsync("pong!");
    }
}