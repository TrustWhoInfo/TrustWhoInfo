using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace backend.Hubs
{
    public class WorldGameHub : Hub
    {
        private readonly World world;

        public WorldGameHub(World world) {
            this.world = world;
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var player = world.GetPlayer(Context.ConnectionId);
            if (player != null) {
                world.RemovePlayer(player);
            }
            return base.OnDisconnectedAsync(exception);
        }

        public async Task RegisterUser(string name) {
            Log.Information("New user registered {username}", name);
            var player = new Player(name, Context.ConnectionId);
            world.AddPlayer(player);
        }

        public async Task Move(float direction)
        {
            var player = world.GetPlayer(Context.ConnectionId);
            if (player != null) {
                player.AddCommand(new MoveCommand(player, direction));
            } else {
                Log.Warning("Unknown player");
            }
        }

        public async Task Fire(float direction)
        {
            var player = world.GetPlayer(Context.ConnectionId);
            if (player != null) {
                player.AddCommand(new FireCommand(player, direction));
            } else {
                Log.Warning("Unknown player");
            }
        }
    }
}