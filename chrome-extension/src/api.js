export default class Api {
    static async postData(url = '', data = {}) {
        try {
            const response = await fetch(url, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(data)
            });
            const text = await response.text();
            const obj = JSON.parse(text);
            return {ok: true, data: obj};
        } catch (ex) {
            return { ok: false, error: ex };
        }
    }

    static async getTrustLevels(code, news) {
        return this.postData('https://localhost:7071/api/trustLevel', {code: code, news: news});
    }
} 