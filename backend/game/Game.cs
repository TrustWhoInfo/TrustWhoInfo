namespace backend;
public class Game {
    public Game() {
        var world = new World();
    }
}

/*

World
    Processor
        Tick
            Every active chunk  
                Every active entity

    Map
        Chunks
            Active: true/false
            Entities: [mobs, players, objects]
    Connections
        Player
            Coordinates [chunk]
            Receive:
                entities updates
                    coordinate updates [x,v,a]
                    actions: [jump,eat,sit,...]
            Send: 
                commands


*/