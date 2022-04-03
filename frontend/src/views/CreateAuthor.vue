<template>
  <div class="createAuthor">
    <div>
      <form>
          <div>
            <h4>Submit new author</h4>
          </div>
          <div class='form-group'>
              <label>Name</label>
              <input type='text' placeholder="Name" v-model='author.name' />
          </div>
          <div class='form-group'>
              <label>Title</label>
              <input type='text' placeholder="Title" v-model='author.title' />
          </div>
          <div class='form-group'>
              <label>Author home URL</label>
              <input type='url' placeholder="URL" v-model='author.url' />
          </div>
          <div class='form-group'>
              <label>lang</label>
              <input type='text' placeholder="Language" v-model='author.lang' />
          </div>
          <div class='form-group'>
              <label>Image</label>
              <input type='text' placeholder="Image" v-model='author.image' />
          </div>
          <div>
              <button @click.prevent='submit'>Submit</button>
          </div>
      </form>
    </div>
  </div>
</template>

<script>
import Api from '@/api';

export default {
  name: 'CreateAuthor',
  components: {    
  },
  data() {
    return {
      author: {
        url: '',
        name: '',
        image: '',
        lang: '',
        title: '',
      },
    };
  },
  methods: {
    async submit() {
      await Api.createAuthor(this.author);
      this.author = {};      
    },
  },
}

</script>

<style lang='scss'>
  .createAuthor {
    padding: 16px;
    .form-group {
      width: 100%;
      margin: 4px;

      label {
        display: inline-block;
        width: 150px;
        text-align: right;
        margin-right: 8px;
      }
    }
  }
</style>
