using System.Collections.Concurrent;

namespace backend;

public class Player: Entity {
    public string ConnectionId {get;set;}
    public ConcurrentQueue<BaseCommand> Commands = new();
    public Dictionary<long, Entity> KnownEntities {get;} = new();

    public Player(string name, string connectionId) : base("player")
    {
        Name = name;
        ConnectionId = connectionId;
    }

    internal void AddCommand(BaseCommand command)
    {
        Commands.Enqueue(command);
    }

    public void ProcessCommands() {

    }
}
