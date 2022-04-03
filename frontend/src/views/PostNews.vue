<template>
  <div class="postNews">
    <div>
      <form>
          <div>
            <h4>Submit news</h4>
          </div>
          <div class='form-group'>
              <label>URL</label>
              <input type='url' placeholder="URL" v-model='article.url' />
          </div>
          <div class='form-group'>
              <label>Title</label>
              <input type='text' placeholder="Title" v-model='article.name' />
          </div>
          <div class='form-group'>
              <label>Subject</label>
              <input type='text' placeholder="Subject" v-model='article.subject' />
          </div>
          <div class='form-group'>
              <label>Media</label>
              <input type='text' placeholder="Media" v-model='article.media' />
          </div>
          <div class='form-group'>
              <label>Author</label>
              <input type='text' placeholder="Author" v-model='article.author' />
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
  name: 'PostNews',
  components: {    
  },
  data() {
    return {
      article: {
        url: '',
        name: '',
        subject: '',
        author_id: -1,
        media_id: -1,
      },
      authors: [],
      media: [],
      subjects: [],
    };
  },
  methods: {
    async submit() {
      await Api.postNews(this.article);
    },
  },
}

</script>

<style lang='scss'>
  .postNews {
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
