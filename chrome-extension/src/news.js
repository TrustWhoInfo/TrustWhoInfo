export default class News {
    constructor(url, media, author, subjects) {
        this.url = url;
        this.media = media;
        this.author = author;
        this.subjects = subjects || [];
    }
}