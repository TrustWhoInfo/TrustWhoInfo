namespace backend;

public class WorldUpdates {
    public PositionUpdate Position {get;set;}
    public List<object> Updates {get;set;}
    public List<EntityUpdate> Entities { get; internal set; }
}


