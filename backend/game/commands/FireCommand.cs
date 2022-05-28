namespace backend;

public class FireCommand : PlayerCommand
{
    public FireCommand(Player player, float direction) : base(player)
    {
        Direction = direction;
    }

    public float Direction {get;}
}