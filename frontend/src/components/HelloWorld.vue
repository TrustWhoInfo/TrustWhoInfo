<template>
  <div>
    <div class="block block-even">
      <div style='display: inline-block; margin-right: 24px;'>
        <img src='/images/favicon.png' style='max-height: 100px' alt='Favicon' />
      </div>
      <div style='display: inline-block'>
        <h1 style='margin:0;margin-bottom:8px;'>Trust Who?</h1>
        <span>Stop censorship via hiding information. Choose who <span style='color: deepskyblue;font-weight: bold'>you</span> trust</span>
      </div>
    </div>
    <div class="block">
      <h3>How this works?</h3>
      <div>
        You set trust level for each news-maker and media. We show "trust level" near every news based on your preference.
      </div>
      <a style='margin-top: 12px;display:inline-block' @click.prevent='diagram=!diagram' href='#'>{{diagram ? 'Hide' : 'Show'}} horrible diagram</a>
      <div class='diagram' v-if='diagram'>
        <img src='images/diagram.png' style='max-width: 100%;' alt='Diagram that explains who Trust Level is built' />
        <div style='#a900ff; border:solid 1px #a900ff; padding: 12px;width:max-content;margin-top: 24px;'>
            Trust Level = 
              <span style='color:gray'>function of</span> 
              `media trust level`, `writer trust level`, `trust level assigned by news analytic`
        </div>              
        <div style='padding: 12px;'>All of above is selected by yourself and yourself only. No central censorship</div>
      </div>

    <hr style='margin-top: 32px;margin-bottom: 24px;' />

    <span style='font-weight: bold; font-size: 1.2em; margin-bottom: 4px;display: inline-block;'>Chose who you trust</span>
    <div>by moving slider left-right</div>
    <br/>

    <div class="block">
      <div>
        <template v-for='author in authors' :key='author.name'>
          <author-avatar :user='author' @click='selectedUser=author' />
        </template>
      </div>
      <person-card :user='selectedUser' v-if='selectedUser' />
    </div>

      <div>
        <div id='your-code'>
          <span class='label'>Your profile code</span>
          <div class='code' v-if='code'>
            {{code}}
          </div>
          <button v-else class='no-code' @click='generateCode'>Generate</button>
        </div>
      </div>
    </div>

    <hr />

  </div>
</template>

<script>
import AuthorAvatar from './AuthorAvatar.vue'
import PersonCard from './PersonCard.vue';
import Api from '@/api';

export default {
  name: 'HelloWorld',
  components: {
    AuthorAvatar, PersonCard,
  },
  props: {
    
  },
  data() {
    return {
      diagram: false,
      selectedUser: null,
      generating: false,
      code: "",
      media: [],
      authors: [],
      users: [
        {
          avatar: "000001",
          name: "Donald Trump",
          title: "Ex-president of USA",
          country: "USA",
          tags: ["Government", "Ex",],
          level: 50,
          levels: {
            "Politics": 30,
            "Business": 90,
            "Sport": 0,
          },
        },
        {
          avatar: "000002",
          name: "Elon Musk",
          title: "Enterpreneur",
          country: "USA",
          tags: ["Business",],
          level: 88,
          levels: {
            "Politics": 10,
            "Business": 100,
            "Sport": -30,
          },
        },
        {
          avatar: "000003",
          name: "Angela Merkel",
          country: "Germany",
          tags: ["Government", "Ex",],
          title: "Ex-canselor of Germany",
          level: -10,
          levels: {
            "Politics": 30,
            "Business": -30,
            "Sport": -70,
          },
       },
        {
          avatar: "000004",
          name: "Vladimir Zelensky",
          country: "Ukraine",
          tags: ["Government", ],
          title: "President of Ukraine",
          level: 30,
          levels: {
            "Politics": 0,
            "Business": -90,
            "Sport": -60,
          },
        },
        {
          avatar: "000005",
          name: "Joe Biden",
          country: "USA",
          tags: ["Government", ],
          title: "President of USA",
          level: -80,
          levels: {
            "Politics": 20,
            "Business": -10,
            "Sport": -90,
          },
        },
      ],
    }
  },
  async mounted() {
    const media = await Api.loadMedia();
    this.media = media;
    console.log("media", media);

    const authors = await Api.loadAuthors();
    authors.forEach(author => {
      author.level = 0;
    })
    this.authors = authors;    
    console.log("authors", authors);
  },
  methods: {
    async generateCode() {
      this.generating = true;
      const code = await Api.generateCode(this.users);
      this.code = code;
      this.generating = false;
    },
  },
}
</script>

<style scoped lang="scss">
.author-avatar {
  margin-right: 12px;
}
.block {
//  margin-bottom: 12px;
  padding: 12px 12px;
  &.block-even {
    background: #f8f8f8;
  }
}
#your-code {
    height: 50px;
    width: 400px;
    position: relative;
    margin: 8px auto;
    display: flex;
    align-content: center;
    align-items: center;

    .label {
      font-size: 1.5em;
      vertical-align: middle;
      color: #9d31ff;
    }

    .code, .no-code {
      display: inline-block;
    }

    .code {      
      height: 34px;
      width: 145px;

      border: solid 1px lightgray;
      text-align: center;
      margin: 0 10px;
      color: #1bb510;
      font-size: 2em;
      padding: 8px;
    }

    .no-code {
      height: 34px;
      width: 145px;
      color: rgb(255, 255, 255);
      margin: 0 10px;
      cursor: pointer;
      position: relative;
      background: linear-gradient(0deg, rgb(147, 82, 252), rgb(114, 54, 253));
    }
}

  @media (min-width: 760px) {
    .diagram {
      padding: 32px;
    }
  }
</style>
