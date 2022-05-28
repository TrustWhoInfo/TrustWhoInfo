using backend.Hubs;
using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace backend;

public class UpdatesService : BackgroundService
{
    private readonly IServiceProvider services;
    private readonly World world;
    private readonly IHubContext<WorldGameHub> hub;

    public UpdatesService(IServiceProvider services, World world, IHubContext<WorldGameHub> hubContext) {
        Log.Information("Created service");
        this.services = services;
        this.world = world;
        this.hub = hubContext;
    }

    private async Task SendPlayerUpdates()
    {
        var players = world.GetPlayers();
        foreach(var player in players) {
            var updates = GetPlayerUpdates(player);
            await SendWordUpdates(player, updates);
        }

        // reset one-time flags
        foreach(var chunk in world.GetActiveChunks()) {
            foreach(var entity in chunk.GetEntities()) {
                entity.NewlyCreated = false;
                if (entity.NewlyDead) {
                    world.AddCommand(new DespawnEntity(entity));
                    entity.NewlyDead = false;
                }
            }
        }
        await Task.Delay(50);
    }
    
    public async Task SendWordUpdates(Player player, WorldUpdates updates) {
        await hub.Clients.Client(player.ConnectionId).SendAsync("WorldUpdates", updates);
    }
    
    private WorldUpdates GetPlayerUpdates(Player player)
    {
        var updates = new WorldUpdates();
        var entities = world.GetEntitiesAroundPlayer(player);
        updates.Entities = entities.Select(entity => GetEntityUpdate(player, entity)).ToList();
        AddDissapearedEntities(updates, player);
        updates.Position = new PositionUpdate(player.Position);
        return updates;
    }

    private void AddDissapearedEntities(WorldUpdates updates, Player player) {
        var dissapeared = new Dictionary<long, Entity>(player.KnownEntities);
        foreach(var update in updates.Entities) {
            dissapeared.Remove(update.Id);
        }
        foreach(var entity in dissapeared.Values) {
            var update = new EntityUpdate();
            update.Id = entity.Id;
            update.EntityDissappeared = new EntityDissappearedDetails{
            };
            updates.Entities.Add(update);

            player.KnownEntities.Remove(entity.Id);
        }
    }

    private EntityUpdate GetEntityUpdate(Player player, Entity entity) {
        var update = new EntityUpdate();
        update.Id = entity.Id;
        if (player == entity)
            update.Me = true;
        update.X = entity.Position.Pos.X;
        update.Y = entity.Position.Pos.Y;
        update.Vx = entity.Position.Vel.X;
        update.Vy = entity.Position.Vel.Y;
        update.Time = entity.Position.Time;

        if (entity.NewlyCreated) {
            update.EntityCreated = new EntityCreatedDetails {
                Reason = "birth",
            };
        }
        if (!player.KnownEntities.ContainsKey(entity.Id)) {
            update.EntityAppeared = new EntityAppearedDetails {
                Name = entity.Name,
                Type = entity.Type,
            };
            player.KnownEntities[entity.Id] = entity;
        }
        if (entity.NewlyDead) {
            update.EntityDeath = new EntityDeathDetails {
                Reason = "age limit",
            };
        }
        return update;
    }


    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var scope = services.CreateAsyncScope();
        //scope.ServiceProvider.GetRequiredService<>();
        while(!cancellationToken.IsCancellationRequested) {
            await SendPlayerUpdates();
        }
    }
}

public class EntityUpdate
{
    public int Id { get; internal set; }
    public float X { get; internal set; }
    public float Y { get; internal set; }
    public float Vx { get; internal set; }
    public float Vy { get; internal set; }
    public int Time { get; internal set; }
    public bool? Me {get;set;}

    public EntityAppearedDetails? EntityAppeared {get;set;}
    public EntityDissappearedDetails? EntityDissappeared {get;set;}
    public EntityCreatedDetails? EntityCreated {get;set;}
    public EntityDeathDetails? EntityDeath {get;set;}
}

public class EntityAppearedDetails {
    public string Name { get; internal set; }
    public string Type { get; internal set; }
}

public class EntityDissappearedDetails {
}

public class EntityCreatedDetails {
    public string Reason { get; internal set; }
}

public class EntityDeathDetails {
    public string Reason {get;set;}
}