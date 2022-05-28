<template>
    <div ref='container3d' tabindex="0" @keydown='keyup' style='width:100%;height:100%;background: black; color: white; display: flex; flex: 1 1;position: relative;'>
    </div>
</template>

<script>
import '../../components/common';
import '../../components/common_vue';
import Vue from 'vue';
import Helper from '../../components/helper';
import Api from '../../components/api';
import { OrbitControls } from 'three/examples/jsm/controls/OrbitControls.js';
import { GLTFLoader } from 'three/examples/jsm/loaders/GLTFLoader.js';
import * as THREE from 'three';
import { FontLoader } from 'three/examples/jsm/loaders/FontLoader.js';
import { TextGeometry } from 'three/examples/jsm/geometries/TextGeometry.js';
import Stats from 'three/examples/jsm/libs/stats.module.js';
import { GUI } from 'three/examples/jsm/libs/lil-gui.module.min.js';
import PhysSpriteObject from './physSpriteObject';
import PhysWorld from './physWorld';

let world = new PhysWorld();

const TWEEN = require('@tweenjs/tween.js');

let canvasWidth, canvasHeight;
let renderer, camera, zoom = 1, font, controls;
const scene = new THREE.Scene();
const pointer = new THREE.Vector2();
let clickedPoint, clickedObject;
let raycaster = new THREE.Raycaster();
let INTERSECTED;
let objects = [];
const lineHighlightGroup = new THREE.Object3D();
const loader = new THREE.TextureLoader();
loader.setPath('/static/images/games/world/');
world.setTextureLoader(loader);
let hero;

export default {
  props: [],
  data() {
    return {
    };
  },
  components: {},
    mounted: function () {
        this.loadFont(()=>{
            this.setupRenderer();
            this.setupScene();
            this.setupLight();
            this.setupCamera();
            this.setupControls();
            this.setupGeometry();

            this.$refs.container3d.addEventListener('mousemove', this.onPointerMove);
            this.$refs.container3d.addEventListener('click', this.onClick);

            this.animate();
        });
    },
    methods: {      
        keyup(e) {
          const code = e.code;
          let forward=0, strife=0, up=0;
          
          if (e.code == 'KeyW') { // top
            forward = 1;
          } else if (e.code == 'KeyS') { // down
            forward = -1;
          } else if (e.code == 'KeyA') { // left
            strife = 1;
          } else if (e.code == 'KeyD') { // right
            strife = -1;
          } else if (e.code == 'Space') { // jump
            up = 1;
          } else {
            console.log("keydown", e);
          }

          const direction = hero.mesh.position.clone();
          direction.sub(camera.position);
          direction.y = 0;
          direction.normalize();
          //forward = 1;strife = 0;
          const dx = forward * direction.x + strife * direction.z;
          const dz = forward * direction.z - strife * direction.x;

          hero.applyForce([dx,0,dz]);

         // let newPos = {x:hero.pos[0]+dx, y:hero.pos[1], z: hero.pos[2]+dz};
         // hero.pos = [newPos.x, newPos.y, newPos.z];

        //  controls.target.set(newPos.x, newPos.y, newPos.z);
        //  controls.update();              
          
        //  const cam = camera.position;
        //  camera.position.set(cam.x + dx, cam.y, cam.z + dz);
        },
        onPointerMove(event) {
            pointer.x = event.offsetX / canvasWidth * 2 - 1;
			      pointer.y = -event.offsetY / canvasHeight * 2 + 1;
        },     
        onClick(event) {
            clickedPoint = new THREE.Vector2();
            clickedPoint.x = event.offsetX / canvasWidth * 2 - 1;
      			clickedPoint.y = -event.offsetY / canvasHeight * 2 + 1;
        },
        setupRenderer() {
            const container = this.$refs.container3d;
            canvasWidth = container.offsetWidth;
            canvasHeight = container.offsetHeight;

            renderer = new THREE.WebGLRenderer( { antialias: true } );
            renderer.setPixelRatio( window.devicePixelRatio );
            renderer.setSize(canvasWidth, canvasHeight);
            renderer.outputEncoding = THREE.sRGBEncoding;
            renderer.shadowMap.enabled = true;
            this.$refs.container3d.appendChild(renderer.domElement);
        },
        setupScene() {            
            scene.add(lineHighlightGroup);
            world.setScene(scene);
        },
        loadFont(callback) {
            const loader = new FontLoader();
            loader.load('/static/fonts/optimer_regular.typeface.json', response => {
                font = response;
                callback();
            });
        },        
        setupCamera() {
            const ratio = canvasWidth / canvasHeight;
            const width = 20;
            const height = width/ratio;
            //camera = new THREE.OrthographicCamera(-width, width, height, -height, 0.01, 10000);
            camera = new THREE.PerspectiveCamera(45 * zoom, canvasWidth / canvasHeight, 0.01, 10000);
            camera.zoom = zoom;

            camera.position.z = 0;
            camera.position.x = 8;
            camera.position.y = 2;
            camera.lookAt(0,0,0);
            camera.updateProjectionMatrix();
        },
        setupControls() {
            controls = new OrbitControls(camera, renderer.domElement);
        },
        setupLight() {
            scene.add( new THREE.AmbientLight( 0x888888 ) );

            const light = new THREE.DirectionalLight( 0xffffff, 1 );
            light.position.set( 10, 20, 200 );
            scene.add(light);
        },
        setupGeometry() {
            const planeSize = 10000;
            const textureSize = 10;

            const texture = loader.load(`Grass_04.png`);
            texture.wrapS = THREE.RepeatWrapping;
            texture.wrapT = THREE.RepeatWrapping;
            texture.repeat.set(planeSize/textureSize, planeSize/textureSize);

            const textureNormals = loader.load(`Grass_04_Nrm.png`);
            textureNormals.wrapS = THREE.RepeatWrapping;
            textureNormals.wrapT = THREE.RepeatWrapping;
            textureNormals.repeat.set(planeSize/textureSize, planeSize/textureSize);

            const geometry = new THREE.PlaneGeometry(planeSize, planeSize);
            geometry.applyMatrix4(new THREE.Matrix4().makeRotationX(-Math.PI/2));            
            const material = new THREE.MeshPhongMaterial( {
              color: 0x888888, 
              side: THREE.DoubleSide, 
              map: texture, 
              normalMap: textureNormals,
              normalScale: new THREE.Vector2(1, 1),
              shininess: 30,
            //  transparent: true,
              depthTest: true,
              depthWrite: false,
              polygonOffset: true,
              polygonOffsetFactor: -4,
              wireframe: false,              
            });
            const plane = new THREE.Mesh(geometry, material);
            plane.position.y = 0;
            scene.add(plane);

            hero = world.createSprite(0,0,'chars/my2.png');
            //hero = this.addSpriteObject(0,0,'chars/my2.png');
            //this.addSpriteObject(0,3,'chars/flower.png');
            //this.addCube(10,1,'cubes/bricks/texture.png',1);
            //this.addCube(20,0,'cubes/bricks/texture.png',10,'box');

            this.setupPhysics();
        },
        setupPhysics() {            
            
        },
        addCube(x,y,texture,size,name) {
            const map = loader.load(texture);
            const geometry = new THREE.BoxGeometry(size,size,size);
            geometry.applyMatrix4(new THREE.Matrix4().makeTranslation(0, size/2, 0));
            const material = new THREE.MeshPhongMaterial({ map: map });
            const mesh = new THREE.Mesh(geometry, material);
            mesh.position.x = x;
            mesh.position.y = 0;
            mesh.position.z = y;
            scene.add(mesh);

            world.add({type:'box', name: name, size:[size,size,size], pos:[x,size/2,y], move:false, world:world});
        },
        addSpriteObject(x,y,sprite) {
            const charTexture = loader.load(sprite);
          	const hero = new THREE.Sprite(new THREE.SpriteMaterial({ 
              map: charTexture, 
              depthTest: true,
              depthWrite: false,
            }));
            hero.center = [0.5, 0];
            hero.position.set(x, 0, y);
            hero.scale.set(1, 1, 1);
            scene.add(hero);
            return hero;
        },
        update() {
            
        },
        updateMouse() {
            raycaster.setFromCamera(pointer, camera);
            const intersects = raycaster.intersectObjects(objects, false);
            if (intersects.length > 0) {
                if (INTERSECTED != intersects[0].object) {
                    if (INTERSECTED) {
                        INTERSECTED.material.emissive.setHex(INTERSECTED.currentHex);
                        if (INTERSECTED != clickedObject) {
                            INTERSECTED.hideText();
                        }
                    }
                    INTERSECTED = intersects[0].object;
                    INTERSECTED.currentHex = INTERSECTED.material.emissive.getHex();
                    INTERSECTED.material.emissive.setHex(0xff0000);
                    INTERSECTED.showText();

                    this.selectedNode = INTERSECTED.node;
                    let pos = new THREE.Vector3();
                    INTERSECTED.getWorldPosition(pos);
                    this.distance = Math.round(camera.position.distanceTo(pos))/1000;
                }

            } else {
                if (INTERSECTED) {
                    INTERSECTED.material.emissive.setHex( INTERSECTED.currentHex );
                    if (INTERSECTED != clickedObject) {
                        INTERSECTED.hideText();
                    }
                }
                INTERSECTED = null;

            }      
            
            if (clickedPoint) {
                raycaster.setFromCamera(clickedPoint, camera);
                const intersects = raycaster.intersectObjects(objects, false);
                if (intersects.length > 0) {
                    clickedObject = intersects[0].object;
                    
console.log("CLICK", clickedObject);

                    this.retarget(clickedObject);
                    //this.showInfo(clickedObject);
                    this.highlightLinks(clickedObject);

                }
                clickedPoint = null;
            }
        },
        animate() {
            world.update();
            this.update();
            TWEEN.update();
            if (hero && hero.position) {
              //camera.position.set(hero.position.clone());
              //camera.position.y = hero.position.y + 10;
              camera.lookAt(hero.position);
            }
            renderer.render(scene, camera);
            requestAnimationFrame(this.animate);
        },
    }
};
</script>

<style lang='scss'>
  html, body, #body-container {
    height: 100%;
    position: relative;
  }
  #body-container {
    display: flex;
  }
</style>

<style lang="scss" scoped>
  @import "@root/common";

  .game-page {
    display: flex;
    flex-direction: column;
    flex: 1 1;
  }

</style>