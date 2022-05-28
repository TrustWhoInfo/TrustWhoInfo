import Phaser from 'phaser';

let platforms, player, cursors, stars, bombs;
let score = 0, scoreText;

export default class TestScene extends Phaser.Scene {    
    constructor() {
        super('test-scene');
    }
    
    preload() {
        this.load.image('sky', '/static/images/games/world/tutorial/sky.png');
        this.load.image('ground', '/static/images/games/world/tutorial/platform.png');
        this.load.image('star', '/static/images/games/world/tutorial/star.png');
        this.load.image('bomb', '/static/images/games/world/tutorial/bomb.png');
        this.load.spritesheet('dude', '/static/images/games/world/tutorial/dude.png', { frameWidth: 32, frameHeight: 48 });
    }

    create() {
        console.log("Creating scene");
        this.add.image(400, 300, 'sky');
      
        platforms = this.physics.add.staticGroup();
      
        platforms.create(400, 568, 'ground').setScale(2).refreshBody();
      
        platforms.create(600, 400, 'ground');
        platforms.create(50, 250, 'ground');
        platforms.create(750, 220, 'ground');
      
        player = this.physics.add.sprite(100, 450, 'dude');
      
        player.setBounce(0.2);
        player.setCollideWorldBounds(true);
      
        this.anims.create({
            key: 'left',
            frames: this.anims.generateFrameNumbers('dude', { start: 0, end: 3 }),
            frameRate: 10,
            repeat: -1
        });
      
        this.anims.create({
            key: 'turn',
            frames: [ { key: 'dude', frame: 4 } ],
            frameRate: 20
        });
      
        this.anims.create({
            key: 'right',
            frames: this.anims.generateFrameNumbers('dude', { start: 5, end: 8 }),
            frameRate: 10,
            repeat: -1
        });
      
        this.physics.add.collider(player, platforms);
        cursors = this.input.keyboard.createCursorKeys();
      
        stars = this.physics.add.group({
            key: 'star',
            repeat: 11,
            setXY: { x: 12, y: 0, stepX: 70 }
        });
      
        stars.children.iterate(function (child) {
            child.setBounceY(Phaser.Math.FloatBetween(0.4, 0.8));
        });    
        this.physics.add.collider(stars, platforms);
        this.physics.add.overlap(player, stars, this.collectStar, null, this);
      
        scoreText = this.add.text(16, 16, 'score: 0', { fontSize: '32px', fill: '#000' });
      
        bombs = this.physics.add.group();
        this.physics.add.collider(bombs, platforms);
        this.physics.add.collider(player, bombs, this.hitBomb, null, this);
      
        console.log("Scene created");
    }

    collectStar (player, star)
    {
        star.disableBody(true, true);
        score += 10;
        scoreText.setText('Score: ' + score);
    
        if (stars.countActive(true) === 0)
        {
            stars.children.iterate(function (child) {
                child.enableBody(true, child.x, 0, true, true);
            });
    
            var x = (player.x < 400) ? Phaser.Math.Between(400, 800) : Phaser.Math.Between(0, 400);
            var bomb = bombs.create(x, 16, 'bomb');
            bomb.setBounce(1);
            bomb.setCollideWorldBounds(true);
            bomb.setVelocity(Phaser.Math.Between(-200, 200), 20);
        }      
    }
    
    hitBomb (player, bomb)
    {
        this.physics.pause();
        player.setTint(0xff0000);
        player.anims.play('turn');
        gameOver = true;
    }
    
    update() {
        const running = cursors.shift.isDown;
        const speed = running ? 300 : 200;
      
        if (cursors.left.isDown)
        {
            player.setVelocityX(-speed);
            player.anims.play('left', true);
        }
        else if (cursors.right.isDown)
        {
            player.setVelocityX(speed);
            player.anims.play('right', true);
        }
        else
        {
            player.setVelocityX(0);
            player.anims.play('turn');
        }
      
        if (cursors.up.isDown)
        {
            player.setVelocityY(-speed);
        }
        if (cursors.up.isUp)
        {
            player.setVelocityY(speed);
        }
    }
}