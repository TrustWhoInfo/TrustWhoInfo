<template>
    <div ref='game' style='width:100%;height:100%;'></div>
</template>

<script>
import Phaser from 'phaser';
import InfiniteScene from './scenes/infiniteScene';

function initializeGame(ui) {  
  const config = {
    pixelArt: true,
    type: Phaser.AUTO,
    width: Math.floor(ui.offsetWidth/2)*2,
    height: Math.floor(ui.offsetHeight/2)*2,
    parent: ui,
    title: "My title",
    // physics: {
    //     default: 'arcade',
    //     arcade: {
    //         debug: false
    //     }
    // },    
    scene: [InfiniteScene],
    //antialias: false
  };
  console.log("Create game with config", config);
  const game = new Phaser.Game(config);
}

export default {
  props: [],
  data() {
    return {
    };
  },
  components: {},
    mounted: function () {
      initializeGame(this.$refs.game);
    },
    methods: { 
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

  .game-page {
    display: flex;
    flex-direction: column;
    flex: 1 1;
  }

</style>