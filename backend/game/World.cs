using System.Collections.Concurrent;
using System.Numerics;
using Serilog;

namespace backend;

public class World 
{
    private ConcurrentDictionary<string, Player> players = new ConcurrentDictionary<string, Player>();
    private ConcurrentDictionary<ChunkId, Chunk> chunks = new ConcurrentDictionary<ChunkId, Chunk>();
    private volatile List<Chunk> activeChunks = new List<Chunk>();
    private readonly ConcurrentQueue<BaseCommand> commands = new ConcurrentQueue<BaseCommand>();

    public World() {
        
    }

    internal void AddCommand(BaseCommand command) {
        commands.Enqueue(command);
    }

    internal BaseCommand? GetCommand() {
        return commands.TryDequeue(out var command) ? command : null;
    }

    internal void AddPlayer(Player player) {
        players[player.ConnectionId] = player;
        AddCommand(new SpawnPlayer(player));
    }

    internal void RemovePlayer(Player player) {
        players.Remove(player.ConnectionId, out var _);
        AddCommand(new DespawnEntity(player));
    }

    internal List<Player> GetPlayers()
    {
        return players.Values.ToList();
    }

    internal List<Chunk> GetActiveChunks()
    {
        return activeChunks;
    }

    internal Player? GetPlayer(string connectionId)
    {
        return players.TryGetValue(connectionId, out var player) ? player : null;
    }

    internal Chunk GetChunk(ChunkId chunkId)
    {
        return chunks.GetOrAdd(chunkId, ChunkGenerator);
    }

    internal Chunk GetChunk(Vector2 pos)
    {
        var chunkId = Chunk.GetChunkId(pos);
        return GetChunk(chunkId);
    }

    internal Chunk GetChunk(Entity entity)
    {
        return GetChunk(entity.Position.Pos);
    }

    internal Chunk GetChunk(Entity entity, int dx, int dy)
    {
        var chunkId = Chunk.GetChunkId(entity.Position.Pos, dx, dy);
        return GetChunk(chunkId);
    }

    internal IEnumerable<Chunk> GetChunksAround(Entity entity, int r) {
        for(var dx=-r;dx<=r;++dx) {
            for(var dy=-r;dy<=r;++dy) {
                yield return GetChunk(entity, dx, dy);
            }
        }
    }

    internal List<Entity> GetEntitiesAroundPlayer(Player player)
    {
        List<Entity> entities = new List<Entity>();
        foreach(var chunk in GetChunksAround(player, 1)) {
            entities.AddRange(chunk.GetEntities());
        }
        return entities;
    }

    private Chunk ChunkGenerator(ChunkId chunkId)
    {
        return new Chunk(chunkId);
    }

    internal void UpdateActiveChunks()
    {
        var activeChunks = new HashSet<Chunk>();
        foreach(var player in GetPlayers()) {
            foreach(var chunk in GetChunksAround(player, 5)) {
                activeChunks.Add(chunk);
            }
        }
        var newChunks = activeChunks.Except(this.activeChunks);
        var oldChunks = this.activeChunks.Except(activeChunks);
        foreach(var newChunk in newChunks) {
            Log.Information("New chunk added {chunk}", newChunk);
            LoadChunk(newChunk);
        }
        foreach(var oldChunk in oldChunks) {
            Log.Information("Old chunk removed {chunk}", oldChunk);
            UnloadChunk(oldChunk);
        }
        this.activeChunks = activeChunks.ToList();
    }

    private void LoadChunk(Chunk chunk)
    {
        
    }

    private void UnloadChunk(Chunk chunk)
    {
        var entities = chunk.GetEntities();
        foreach(var entity in entities) {

        }
    }
}
