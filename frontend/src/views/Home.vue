<template>
  <div class="home">
    <game />
  </div>
</template>

<script>
import Game from '@/components/main.vue'

export default {
  name: 'Home',
  components: {
    "game": Game,
  }
}
</script>


<style lang='scss'>
  .home {
    width: 100%;
    height: 100%;
  }
</style>