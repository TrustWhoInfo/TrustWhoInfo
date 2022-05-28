using backend.Hubs;

namespace backend;

public class Processor {
    private World world;
    private WorldGameHub hub;

    public Processor() {

    }

    public void RunWorldTick() {
        var chunks = world.GetActiveChunks();
        foreach(var chunk in chunks) {
            var entities = chunk.GetEntities();
            foreach(var entity in entities) {
                RunEntityTick(entity);
            }
        }
    }

    private void RunEntityTick(object entity)
    {
        
    }

    private void SendUpdates() {
        
    }

    public void GameCycle() {
        while(true) {
            RunWorldTick();
        }
    }

    public void SendUpdatesLoop() {

    }
}