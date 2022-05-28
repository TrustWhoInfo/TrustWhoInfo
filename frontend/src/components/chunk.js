import {noise} from './noise/perlin';

let chunkCounter = 0;

export default class Chunk {
    constructor(scene, layer, x, y) {
      this.scene = scene;
      this.x = x;
      this.y = y;
      this.tiles = scene.add.group();
      this.isLoaded = false;
      this.layer = layer;
      const map = scene.make.tilemap({ tileWidth: Chunk.TILE_SIZE, tileHeight: Chunk.TILE_SIZE, width: Chunk.TILES, height: Chunk.TILES});
      this.map = map;
    }
  
    unload() {
      if (this.isLoaded) {
        this.tiles.clear(true, false);
        this.isLoaded = false;
        this.layer1.destroy();
      }
    }
  
    load() {
      if (!this.isLoaded) {
        const image = this.map.addTilesetImage('level1',null,16,16,0,0);
        this.layer1 = this.map.createBlankLayer('layer' + (++chunkCounter), image, this.x * Chunk.SIZE, this.y * Chunk.SIZE);
  
        for (let x = 0; x < Chunk.TILES; x++) {
          for (let y = 0; y < Chunk.TILES; y++) {
  
            let tileX = this.x * Chunk.SIZE + x * Chunk.TILE_SIZE;
            let tileY = this.y * Chunk.SIZE + y * Chunk.TILE_SIZE;
  
            let perlinValue = noise.perlin2(tileX / 250, tileY / 250);
  
            if (perlinValue < -0.5) {
              this.layer1.putTileAt(2,x,y);
            }
            else if (perlinValue < 0.3) {
              this.layer1.putTileAt(1,x,y);
            }
            else if (perlinValue >= 0.3) {
              this.layer1.putTileAt(0,x,y);
            }
          }
        }
  
        this.isLoaded = true;
      }
    }
  }
  

Chunk.TILE_SIZE = 16;
Chunk.TILES = 16;
Chunk.SIZE = Chunk.TILE_SIZE * Chunk.TILES;