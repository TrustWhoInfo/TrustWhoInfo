import Phaser from 'phaser';
import Chunk from '../chunk';
import Player from '../player';
import * as SignalR from "@microsoft/signalr";
import World from '@/world/world';
import UiEntity from '../uiEntity';
import consts from '../consts';


const myId = `TestUser`;

var connection = new SignalR.HubConnectionBuilder().withUrl("/worldGame").build();
connection.on("ReceiveMessage", function (user, message,x,y,z,a,b,c) {
  console.log("ReceiveMessage received", user, message,x,y,z,a,b,c);
});

console.log("world", World);

connection.start().then(function () {
  console.log("Connected to real-time server");

  connection.invoke("RegisterUser", myId).then(result => {
    console.log("User registered", result);
  }).catch(function (err) {
    console.log("Failed to register user", myId);
    return console.error(err.toString());
  }); 

}).catch(function (e) {
  return console.error("Connection to real-time server failed ", e);
});

export default class InfiniteScene extends Phaser.Scene {
    constructor() {
      super({ key: "InfiniteScene" });

      this.entities = {
      };
      this.keysTimers = {
          move: {}, 
          fire: {timeout: 500},
      };
      this.prepareKeyTimers();
    }

    prepareKeyTimers() {
        Object.keys(this.keysTimers).forEach(timer => {
            var obj = this.keysTimers[timer];
            obj.last = 0;
            if (!obj.timeout) obj.timeout = 50;
            obj.mark = () => {
                obj.last = performance.now();
            };
            obj.ready = () => {
                return performance.now() - obj.last > obj.timeout;
            };
        });
    }
  
    preload() {
      this.load.spritesheet("level1", "/static/images/games/world/level1.png", {
        frameWidth: 16,
        frameHeight: 16,
        margin: 0,
        spacing: 0,
      });
      //this.load.image("sprSand", "/static/images/games/world/tutorial/sprSand.png");
      //this.load.image("sprGrass", "/static/images/games/world/tutorial/sprGrass.png");
      this.load.spritesheet('dude', '/static/images/games/world/tutorial/dude.png', { frameWidth: 32, frameHeight: 48 });
      this.load.spritesheet('bomb', '/static/images/games/world/tutorial/bomb.png', { frameWidth: 14, frameHeight: 14 });
      //this.load.tilemapTiledJSON('map', '/static/maps/testmap.json');
    }
  
    create() {  
      this.anims.create({
        key: "sprWater",
        frames: this.anims.generateFrameNumbers("level1", {start: 2, end: 4}),
        frameRate: 5,
        repeat: -1
      });
  
      this.chunkSize = 16;
      this.tileSize = 16;
      this.cameraSpeed = 10;
  
      this.followPoint = new Phaser.Math.Vector2(0, 0);
  
      this.chunkLayer = this.add.group();
      this.chunks = [];
      this.chunkPool = [];
  
      this.keyW = this.input.keyboard.addKey(Phaser.Input.Keyboard.KeyCodes.W, true, true);
      this.keyS = this.input.keyboard.addKey(Phaser.Input.Keyboard.KeyCodes.S, true, true);
      this.keyA = this.input.keyboard.addKey(Phaser.Input.Keyboard.KeyCodes.A, true, true);
      this.keyD = this.input.keyboard.addKey(Phaser.Input.Keyboard.KeyCodes.D, true, true);
      this.keySpace = this.input.keyboard.addKey(Phaser.Input.Keyboard.KeyCodes.SPACE, true, true);

      connection.on("WorldUpdates", updates => {
        World.processServerUpdate(updates);
      });

      this.setupRemoteHandlers() 

    }

    processKeys() {
        const keys = [this.keyW, this.keyS, this.keyA, this.keyD];
        if (keys.filter(c=>c.isDown).length>0 && this.keysTimers.move.ready()) {
            const angle = this.getAngle(...keys);
            connection.invoke("Move", angle);
            this.keysTimers.move.mark();
        }

        if (this.keySpace.isDown && this.keysTimers.fire.ready()) {
            connection.invoke("Fire");
            this.keysTimers.fire.mark();
        }
    }

    getAngle(up,down,left,right) {
        const dx = left.isDown ? 1 : right.isDown ? -1 : 0;
        const dy = up.isDown ? -1 : down.isDown ? 1 : 0;
        return Math.atan2(dx,dy);
    }

    despawn(entity) {
      entity.sprite.destroy();
    }

    getEntitySprite(type, pos) {
        if (type === 'player')
            return new Player(this, pos);
        if (type === 'bullet')
            return new Phaser.GameObjects.Sprite(this, pos.x, pos.y, 'bomb');      
        // default sprite
        return new Phaser.GameObjects.Sprite(this, pos.x, pos.y, 'dude');
    }

    setupRemoteHandlers() {
      connection.on("move", function (timestamp, entityId, x, y, vx, vy) {
        console.log("move", timestamp, entityId, x, y, vx, vy);
      });
    }
  
    getChunk(x, y) {
      for (var i = 0; i < this.chunks.length; i++) {
        if (this.chunks[i].x == x && this.chunks[i].y == y) {
          return this.chunks[i];
        }
      }
      return null;
    }

    syncEntities() {
        while(World.entitiesAppeared.length>0) {
            const e = World.entitiesAppeared.pop();
            const sprite = this.getEntitySprite(e.type, e.pos);
            const uiEntity = new UiEntity(e, sprite);
            this.entities[uiEntity.entity.id] = uiEntity;
            this.add.existing(sprite);
        }
        while(World.entitiesDissappeared.length>0) {
            const e = World.entitiesDissappeared.pop();
            var uiEntity = this.entities[e.id];
            uiEntity.sprite.destroy();
            delete this.entities[e.id];
        }
    }
  
    update(t,dt) {
      const chunkWidth = this.chunkSize * this.tileSize;

      let playerChunkX = Math.round((this.followPoint.x - chunkWidth/2) / chunkWidth);
      let playerChunkY = Math.round((this.followPoint.y - chunkWidth/2) / chunkWidth);
  
      const sizeToLoadX = 4;
      const sizeToLoadY = 3;
      for (let x = playerChunkX - sizeToLoadX; x <= playerChunkX + sizeToLoadX; x++) {
        for (let y = playerChunkY - sizeToLoadY; y <= playerChunkY + sizeToLoadY; y++) {
          const existingChunk = this.getChunk(x, y);
  
          if (existingChunk == null) {
            let newChunk = this.chunkPool.pop() ?? new Chunk(this, this.chunkLayer, x, y);
            newChunk.x = x;
            newChunk.y = y;
            this.chunks.push(newChunk);            
            //console.log(`Created chunk ${x},${y}. Totally ${this.chunks.length} chunks loaded`);
          }
        }
      }
  
      for (let i = 0; i < this.chunks.length; i++) {
        const chunk = this.chunks[i];
  
        const distanceX = Math.abs(chunk.x - playerChunkX);
        const distanceY = Math.abs(chunk.y - playerChunkY);
        if (distanceX <= sizeToLoadX && distanceY <= sizeToLoadY) {
            chunk.load();
        } else {
            chunk.unload();
            this.chunks.splice(i, 1);
            this.chunkPool.push(chunk);
            i--;
            //console.log(`Unloading chunk ${chunk.x},${chunk.y} due to long distance ${distanceX},${distanceY}. Totally ${this.chunks.length} chunks loaded`);
        }
      }

      this.syncEntities();

      Object.values(this.entities).forEach(uiEntity => {
        const e = uiEntity.entity;
        e.pos.x = e.server.x;
        e.pos.y = e.server.y;        
        uiEntity.update();
      });

      // if (animation) {
      //   this.player.anims.play(animation, true);
      // }
        
      if (World.player) {
        this.cameras.main.centerOn(World.player.pos.x * consts.PIXELS_IN_METER, World.player.pos.y * consts.PIXELS_IN_METER);
      }

      this.processKeys();
    }
  }