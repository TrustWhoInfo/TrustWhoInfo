<template>
  <div class="person-card">
    <div class='names'>
      <div class='name'><b>Name:</b> {{user.name}}</div>
      <div class='title'><b>Title:</b> {{user.title}}</div>
      <div class='tags'><b>Tags:</b> {{user.tags.join(', ')}}</div>
    </div>
    <div v-if='user.levels' style='margin-top: 24px'>
      <div style='margin-bottom: 12px;'>
        <b>Per-subject trust level</b>
        <span style='padding-left: 12px; color: #888; font-size: 0.99em;'>Allows to finely adjust level of trust to this source for each news subject</span>
      </div>
      <template v-for='key in Object.keys(user.levels)' :key='key'>
        <div>
          <span class='area'>{{key}}</span>
          <trust-bar :level='user.levels[key]' @onlevel='e=>onlevel(e,key)' style='' :horizontal="true" />
        </div>
      </template>
      <div>
        <span class='area'>Other</span>
        <trust-bar :level='user.level' @onlevel='e=>onlevel(e)' style='' :horizontal="true" />
      </div>
    </div>
  </div>
</template>

<script>
import TrustBar from './TrustBar.vue'
export default {
  name: 'PersonCard',
  components: {
    TrustBar
  },
  props: {
    user: Object,
  },
  methods: {
    onlevel(e,key) {
      const u = this.user;
      if (key) {
        u.levels[key] = +e;
      } else {
        u.level = +e;
      }
    }
  }
}
</script>

<style scoped lang="scss">
.person-card {
  display: block;
  padding: 12px;
  background: #eee;
  width: 100%;
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
}

.name {
  
}

.title {
  
}

.area {
  min-width: 100px;
  display: inline-block;
}
</style>
