import News from "../news";
import Provider from "../provider"

const media = 'tass.ru';

export default class Tass extends Provider {
    install() {
        this.addPreviewIndicators();
        this.addNewsIndicator();
    }

    async addNewsIndicator() {
        const articles = document.querySelectorAll('.news');
        
        for(let i=0;i<articles.length;++i) {
            const article = articles[i];
            const news = this.getNews(article);
            const levels = await this.getTrustLevels([news]);
            if (levels.ok && levels.data.ok) {
                const level = levels.data.levels[0];
                const ui = this.createPreviewIndicator(level);
                const topHeader = article.querySelector('.news-header__top');
                if (topHeader) {
                    topHeader.appendChild(ui);
                }
            }
        }
    }

    getSubjectsFromUrl(url) {
        if (!url) return [];
        const pathIndex = url.indexOf(media + '/') + media.length + 1;
        const subjIndex = url.indexOf('/', pathIndex + 1);
        const subject = url.substr(pathIndex, subjIndex - pathIndex);
        return [subject];
    }

    getNews(ui) {
        const url = ui.dataset.ioArticleUrl;
        console.log(ui, url);
        const author = null;
        const subjects = this.getSubjectsFromUrl(url);
        return new News(url, media, author, subjects);
    }

    async addPreviewIndicators() {
        const previews = document.querySelectorAll('a.news-preview');
        const news = [];
        for(let i=0;i<previews.length;++i) {
            const preview = previews[i];
            const url = preview.href;
            const subjects = this.getSubjectsFromUrl(url);
            news.push(new News(url, media, null, subjects));
        }
        const trustLevels = await this.getTrustLevels(news);
        if (trustLevels.ok && trustLevels.data.ok) {
            for (let i=0;i<previews.length;++i) {
                const trustLevel = trustLevels.data.levels[i];
                const preview = previews[i];                
                const ui = this.createPreviewIndicator(trustLevel);
                const body = preview.querySelector('.news-preview__body');
                if (body) {
                    body.appendChild(ui);
                }
            }
        }
    }
}