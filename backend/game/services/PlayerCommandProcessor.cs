using backend.Hubs;
using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace backend;

public class PlayerCommandProcessor : BackgroundService
{
    private readonly IServiceProvider services;
    private readonly World world;

    public PlayerCommandProcessor(IServiceProvider services, World world) {
        this.services = services;
        this.world = world;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while(!cancellationToken.IsCancellationRequested) {
            var players = world.GetPlayers();
            foreach(var player in players) {
                ProcessCommands(player);
            }
            await Task.Delay(50);
        }
    }

    private void ProcessCommands(Player player)
    {
        for(var i=0;i<10;++i) {
            if (!player.Commands.TryDequeue(out var command)) break;            
            if (command is MoveCommand moveCommand) {
                moveCommand.Execute(player);                
            } else {
                world.AddCommand(command);
            }
        }
    }
}
