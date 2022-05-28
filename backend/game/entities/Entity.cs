using System.Numerics;

namespace backend;

public class Entity {
    public Entity(string type) {
        Type = type;
        Created = DateTime.UtcNow;
        NewlyCreated = true;
    }

    public DateTime Created {get;}
    public long Age {get;set;} = -1;
    public bool NewlyCreated {get;set;}
    public bool NewlyDead {get;set;}

    public long GetAge(DateTime now) {
        return (long)(now - Created).TotalMilliseconds;
    }

    public bool Dead {get;set;}

    public void MakeDead() {
        Dead = true;
        NewlyDead = true;
    }

    private static int ID_COUNTER;
    public int Id {get;} = Interlocked.Increment(ref ID_COUNTER);
    public string Name {get;set;}
    public string Type {get;}
    public PositionState Position {get;} = new PositionState();
    public float Mass { get; internal set; } = 1f;

    public override string ToString()
    {
        return $"{Type}:{Name}@{Id}";
    }
}

public class State {
    public bool Dirty {get;set;}
}

public class PositionState : State {
    public float MaxVelocity {get;set;} = 1f;
    public Vector2 Pos {get;set;} = new Vector2();
    public Vector2 Vel {get;set;} = new Vector2();
    public Vector2 Acc {get;set;} = new Vector2();
    public int Time {get;set;}
}

public class PositionUpdate {
    public PositionUpdate(PositionState state) {
        Pos = state.Pos;
        Vel = state.Vel;
        Time = state.Time;
    }

    public Vector2 Pos {get;set;}
    public Vector2 Vel {get;set;}
    public int Time {get;set;}
}
