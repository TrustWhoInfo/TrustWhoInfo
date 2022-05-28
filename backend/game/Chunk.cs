using System.Collections.Concurrent;
using System.Numerics;
using Serilog;

namespace backend;

public class Chunk
{
    public static readonly int SIZE_POW = 4;
    public static readonly int SIZE = 1 << SIZE_POW;

    internal List<Entity> GetEntities(bool includeDead = false)
    {
        return entities.Values.Where(c=>!c.Dead || includeDead).ToList();
    }

    private ConcurrentDictionary<int, Entity> entities = new ConcurrentDictionary<int, Entity>();
    private ChunkId chunkId;

    public Chunk(ChunkId chunkId)
    {
        this.chunkId = chunkId;
    }

    public void AddEntity(Entity entity) {
        Log.Information("Entity {entity} added to chunk {chunkId}", entity, chunkId);
        entities[entity.Id] = entity;
    }

    public void RemoveEntity(Entity entity) {
        Log.Information("Entity {entity} removed from chunk {chunkId}", entity, chunkId);
        entities.Remove(entity.Id, out var _);
    }

    internal static ChunkId GetChunkId(Vector2 pos, int dx=0, int dy=0)
    {
        var x = (int) Math.Floor(pos.X) >> SIZE_POW;
        var y = (int) Math.Floor(pos.Y) >> SIZE_POW;
        return new ChunkId(x+dx,y+dy);
    }

    public override string ToString()
    {
        return chunkId.ToString();
    }
}

public readonly record struct ChunkId(int X, int Y);



