let TIME_QUANT = 0.050;

class World {
    constructor() {
        this.entities = {};
        this.chunks = [];
        this.entitiesCreated = [];
        this.entitiesDead = [];
        this.entitiesAppeared = [];
        this.entitiesDissappeared = [];

        this.player = null;
    }

    processServerUpdate(updates) {
        const now = performance.now();
        updates.entities.forEach(serverEntity => {          
            let e;
            if (serverEntity.entityAppeared) {
                e = {
                    id: serverEntity.id,
                    new: true,
                    target: new Phaser.Math.Vector2(serverEntity.x, serverEntity.y),
                    pos: new Phaser.Math.Vector2(serverEntity.x, serverEntity.y),
                    v: new Phaser.Math.Vector2(0, 0),
                    server: serverEntity,
                    type: serverEntity.entityAppeared.type,
                    name: serverEntity.entityAppeared.name,
                };
                this.entities[serverEntity.id] = e;
                this.entitiesAppeared.push(e);
                console.log("New entity appeared", e);
            } else {
                e = this.entities[serverEntity.id];
                e.server = serverEntity;
            }
            if (!e) {
                console.error("Unknown server entity received", e);
                return;
            }

            if (serverEntity.entityDissappeared) {
                console.log("entity dissapeared", e);
                this.entitiesDissappeared.push(e);
            }

            if (serverEntity.entityCreated) {
                e.createdReason = serverEntity.entityCreated.reason;
                this.entitiesCreated.push(e);
            }
            if (serverEntity.entityDead) {
                this.entitiesDead.push(e);
            }

            if (serverEntity.me) {
                this.player = e;
            }

            e.targetTime = now + TIME_QUANT;
            e.target.x = serverEntity.x;// + serverEntity.vx * TIME_QUANT;
            e.target.y = serverEntity.y;// + serverEntity.vy * TIME_QUANT;
            e.v.x = (e.target.x - e.pos.x) / TIME_QUANT;
            e.v.y = (e.target.y - e.pos.y) / TIME_QUANT;

            //console.log("entity", e.id, e.pos.x, e.pos.y);

            serverEntity.received = now;
            e.server = serverEntity;
        });
    }
}

const world = new World();
console.log("Create new world", world);

export default world;