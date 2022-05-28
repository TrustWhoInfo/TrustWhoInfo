export default class PhysObject {
    constructor(world, mesh) {
        this.world = world;
        this.mesh = mesh;
        this.forces = [];
        this.acc = [0,0,0];
        this.vel = [0,0,0];
        this.friq = 0.7;
        this.maxVel = 1;
        this.pos = [mesh.position.x,mesh.position.y,mesh.position.z];
        this.move = true;
        this.t = performance.now();
        this.size = [0.5, 1]; // half-width, height
    }

    applyForce(vector) {
        this.forces.push(vector);
    }

    update() {
        let t2 = performance.now();
        const dt = (t2 - this.t)/1000; // seconds
        this.t = t2;

        // initial
        const v = [...this.vel];
        let a = [0,-9.8,0];
        const p = [...this.pos];

        if (this.forces.length > 0) {
            for(let i=0;i<this.forces.length;++i) {
                const force = this.forces[i];
                a = [a[0] + force[0], a[1] + force[1], a[2] + force[2]];
            }
            this.forces = [];
        }
        const v2 = [v[0] + a[0]*dt, v[1] + a[1]*dt, v[2] + a[2]*dt]; // new velocity
        const v3 = [v2[0]*this.friq, v2[1]*this.friq, v2[2]*this.friq]
        const p2 = [p[0] + v3[0]*dt, p[1] + v3[1]*dt, p[2] + v3[2]*dt]; // new position

        const p3 = this.world.canMove(p, p2, size);

        if (this.world.isFree(p2)) {
            console.log(p2);
            this.vel = v3;
            this.pos = p2;
            this.mesh.position.set(...p2);
        }
    }
}