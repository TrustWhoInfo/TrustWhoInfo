using System.Numerics;

namespace backend;

public class MoveCommand : PlayerCommand {
    public MoveCommand(Player player, float direction) : base(player) {
        Direction = direction;
    }

    public float Direction { get; private set; }

    public float DefaultVelocity {get;set;} = 4f;

    internal void Execute(Entity entity)
    {
        var position = entity.Position;
        var rotation = Matrix3x2.CreateRotation(Direction);
        position.Vel = Vector2.Transform(new Vector2(0, DefaultVelocity), rotation);
        position.Time = 1; // ticks
    }
}
