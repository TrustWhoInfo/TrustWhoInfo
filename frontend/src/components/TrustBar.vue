<template>
  <div :class="{'trust-bar':true, horizontal: horizontal}">
    <div class='slider-bar'>
      <input type="range" min="-100" max="100" :value="level" @change="levelUpdate" class="slider">
      <div class='bar'></div>
    </div>
    <div class='label' :style="{color: color(level), textShadow: '1px 1px 1px gray'}">{{label(level)}}</div>    
  </div>
</template>

<script>
export default {
  name: 'TrustBar',
  props: {
    level: Number,
    horizontal: Boolean,
  },
  methods: {
    levelUpdate(e) {
      this.$emit('onlevel', e.target.value);
    },
    label(level) {
      if (level > 80) {
        return "Truth-speaker";
      }
      else if (level > 50) {
        return "Trust";
      }
      else if (level > 20) {
        return "Positive";
      }
      else if (level > -20) {
        return "Neutral";
      }
      else if (level > -50) {
        return "Negative";
      }
      else if (level > -80) {
        return "Distrust";
      }
      else {
        return "Fake-maker";
      }
    },
    color(level) {
      const normalized = (level + 100) / 200; // 0..1      
      const redHue = 0;
      const greenHue = 114;
      const hue = redHue + normalized * (greenHue - redHue);
      return `hsl(${hue}deg, 100%, 50%)`;
    },
  }
}
</script>

<style scoped lang="scss">
.trust-bar {
  width: 100px;
  display: inline-block;
}
.slider {
  width: 100px;
}
.bar {
  width: 100px;
  height: 2px;
  margin: auto;
  background: linear-gradient(90deg, red, lime);
  position: relative;
  top: -4px;
  left: 2px;
  margin-bottom: 4px;
}

.trust-bar.horizontal {
  width: auto;
  
  .slider-bar {
    display: inline-block;
    margin: 0 12px;
  }    
  .label {
    display: inline-block;
  }
}
</style>
