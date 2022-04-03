<template>
  <div class="author-avatar">
    <div class='avatar'>
      <img :src='`${user.image}`' width='118' />
    </div>
    <div class='names'>
      <div class='name'>{{user.name}}</div>
      <div class='title'>{{user.title}}</div>
    </div>
    <trust-bar :level='user.level' @onlevel='onlevel' style='' />
  </div>
</template>

<script>
import TrustBar from './TrustBar.vue';

export default {
  name: 'AuthorAvatar',
  components: {
    TrustBar
  },
  props: {
    user: Object,
  },
  methods: {
    onlevel(e) {
      const u = this.user;      
      u.level = +e;
    }
  }
}
</script>

<style scoped lang="scss">
.author-avatar {
  display: inline-block;
  padding: 12px;
  width: 120px;
  border: solid 1px #ededed;
  text-align: center;
  //background: green;
  &:hover {
    background: #eee;
  }
}

.avatar {
  width: 118px;
  height: 144px;
  border-radius: 16px;
  overflow: hidden;
  margin: auto;
}

.names {
  height: 60px; 
  display: flex; 
  flex-direction: column;
  justify-content: center;
  margin: 0 -12px;
}

.name {
  color: #666;
}

.title {
  font-size: 0.8em;
  color: #888;
}
</style>
