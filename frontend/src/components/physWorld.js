import PhysObject from "./physObject";
import * as THREE from 'three';

// 16x16xN blocks
class Chunk {
    constructor(chunkX, chunkZ) {
        this.layers = {};
        this.chunkX = chunkX;
        this.chunkZ = chunkZ;
    }

    getBlock(x,y,z) {
        const lx = x % this.chunkX;
        const lz = z % this.chunkZ;

        const layer = this.layers[y] ?? {};
    }
}

export default class PhysWorld {
    constructor() {
        this.map = {};
        this.objects = [];
    }

    setTextureLoader(loader) {
        this.loader = loader;
    }

    setScene(scene) {
        this.scene = scene;
    }

    createSprite(x,y,texturePath,size) {
        const texture = this.loader.load(texturePath);
        const sprite = new THREE.Sprite(new THREE.SpriteMaterial({ 
            map: texture, 
            depthTest: true,
            depthWrite: false,
        }));
        sprite.center = [0.5, 0];
        sprite.position.set(x, 0, y);
        size = size ?? 1;
        sprite.scale.set(size, size, size);

        const obj = new PhysObject(this, sprite);
        this.add(obj);
        this.scene.add(sprite);
        return obj;
    }

    add(obj) {
        this.objects.push(obj);
    }

    update() {
        this.objects.forEach(obj => {
            obj.update();
        });
    }

    objects(x,y,z) {
        let x0 = Math.trunc(x);
        let y0 = Math.trunc(y);
        let z0 = Math.trunc(z);
    }

    canMove(p1, p2, size) {
        const x1 = p1[0], y1 = p1[1], z1 = p1[2];
        const x2 = p2[0], y2 = p2[1], z2 = p2[2];
    }

    isFree(pos) {
        const x = pos[0], y = pos[1], z = pos[2];
        if (y < 0) return false;
        return true;
    }
}