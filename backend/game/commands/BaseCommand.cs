namespace backend;

public class BaseCommand {
}

public class PlayerCommand : BaseCommand {
    public PlayerCommand(Player player) {
        Player = player;
    }

    public Player Player {get;}
}