using System.Diagnostics;
using System.Numerics;
using backend.Hubs;
using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace backend;

public class WorldProcessor : BackgroundService
{
    private readonly IServiceProvider services;
    private readonly World world;

    public static readonly int TIME_QUANT = 50;

    public WorldProcessor(IServiceProvider services, World world) {
        this.services = services;
        this.world = world;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var watch = new Stopwatch();

        while(!cancellationToken.IsCancellationRequested) {
            var elapsed = (int) watch.ElapsedMilliseconds;
            watch.Restart();

            var sleepDelay = Math.Max(0, TIME_QUANT - elapsed);
            await Task.Delay(sleepDelay);

            var dt = elapsed + sleepDelay; // note, it could be more than TIME_QUANT

            WorldSideEffects result = ProcessWorldCommands();
            result |= ProcessMovement(dt);
            result |= ProcessDeaths();

            if (result.HasFlag(WorldSideEffects.UpdateActiveChunks)) {
                world.UpdateActiveChunks();
            }

        }
    }

    private WorldSideEffects ProcessDeaths() {
        var result = WorldSideEffects.None;
        var chunks = world.GetActiveChunks();
        var now = DateTime.UtcNow;
        foreach(var chunk in chunks) {
            var entities = chunk.GetEntities();
            foreach(var entity in entities) {
                if (entity.Age > 0)
                    entity.Age--;
                result |= ProcessDeath(entity, now);
            }
        }
        return result;
    }

    private WorldSideEffects ProcessDeath(Entity entity, DateTime now)
    {
        if (entity.Age == 0) {
            entity.MakeDead();
        }
        return WorldSideEffects.None;
    }

    private WorldSideEffects ProcessWorldCommands()
    {
        WorldSideEffects result = WorldSideEffects.None;
        while(true) {
            var command = world.GetCommand();
            if (command == null) break;
            result |= ProcessWorldCommand(command);
        }
        return result;
    }

    private WorldSideEffects ProcessWorldCommand(BaseCommand command)
    {
        var result = WorldSideEffects.None;
        if (command is SpawnPlayer spawnPlayer) {
            Log.Information("Spawn {entity}", spawnPlayer.Player);
            Spawn(spawnPlayer.Player);
            result |= WorldSideEffects.UpdateActiveChunks;
        } else if (command is DespawnEntity despawnEntity) {
            Log.Information("Despawn entity {entity}", despawnEntity.Entity);
            world.GetChunk(despawnEntity.Entity.Position.Pos).RemoveEntity(despawnEntity.Entity);
            result |= WorldSideEffects.UpdateActiveChunks;
        } else if (command is FireCommand fireCommand) {
            Log.Information("Fire bullet at {entity} by {player}", fireCommand.Player.Position.Pos, fireCommand.Player);
            var pos = fireCommand.Player.Position.Pos;
            var rotation = Matrix3x2.CreateRotation(fireCommand.Direction);
            var maxSpeed = 1f;
            var velocity = Vector2.Transform(new Vector2(0, maxSpeed), rotation);
            var bullet = new Bullet(fireCommand.Player.Position.Pos, velocity);
            Spawn(bullet);
        }
        return result;
    }

    private void Spawn(Entity entity) {
        Log.Information("Spawn {entity}", entity);
        world.GetChunk(entity.Position.Pos).AddEntity(entity);
    }

    private float GetFriction(Entity entity) {
        if (entity is Player) return 0.5f;
        return 1;
    }

    private WorldSideEffects ProcessMovement(int dt)
    {
        WorldSideEffects result = WorldSideEffects.None;
        var chunks = world.GetActiveChunks();
        foreach(var chunk in chunks) {
            var entities = chunk.GetEntities();
            foreach(var entity in entities) {
                result |= ProcessMovement(dt, entity);
            }
        }
        return result;
    }

    private WorldSideEffects ProcessMovement(int dt, Entity entity)
    {
        WorldSideEffects result = WorldSideEffects.None;

        var pos = entity.Position;            
        pos.Dirty = false;

        if (entity.Position.Time == 0) {
            pos.Vel = Vector2.Zero;
        }

        if (pos.Vel != Vector2.Zero && entity.Position.Time > 0){
            var prevChunkId = Chunk.GetChunkId(pos.Pos);

            entity.Position.Time--;

            if (pos.Vel != Vector2.Zero) {
                pos.Pos += pos.Vel * dt / 1000f;
                //Log.Information("Entity {name}@{id} moved to {x},{y} with velocity={velocity}", entity.Name, entity.Id, pos.Pos.X, pos.Pos.Y, pos.Vel);
            }

            var newChunkId = Chunk.GetChunkId(pos.Pos);

            if (prevChunkId != newChunkId) {
                var prevChunk = world.GetChunk(prevChunkId);
                var newChunk = world.GetChunk(newChunkId);
                prevChunk.RemoveEntity(entity);
                // hypotethically, entity could dissapear for a moment from players list
                // let's consider it is being a world feature :)
                newChunk.AddEntity(entity);

                if (entity is Player) {
                    result |= WorldSideEffects.UpdateActiveChunks;
                }

                // todo: load/unload chunks
            }

            pos.Dirty = true;
        }
        return result;
    }

}

[Flags]
enum WorldSideEffects {
    None,
    UpdateActiveChunks,
}