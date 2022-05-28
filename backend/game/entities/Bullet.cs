using System.Numerics;

namespace backend;

public class Bullet : Entity
{
    public Bullet(Vector2 pos, Vector2 vel) : base("bullet")
    {
        Position.Pos = pos;
        Position.Vel = vel;
        Position.Time = 100;

        Age = Position.Time + 10;
    }
}
