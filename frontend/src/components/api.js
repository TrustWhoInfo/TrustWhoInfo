export default class Api 
{

    static async postData(url = '', data = {}) {
        try {
            const response = await fetch(url, {
                method: 'POST', // *GET, POST, PUT, DELETE, etc.
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(data)
            });
            let text = await response.text();
            if (!response.ok) {
                if (response.status == 401) {
                    Api.showError('You do not have permission to perform this action');
                } else {
                    console.error("Failed to fetch", url, data, response);
                    Api.showError('Failed to process request<br/> Please refresh the page and try once more');
                }
            }
            try {
                let obj = JSON.parse(text);
                if (url != '/api/event') // do not log events
                    console.log("POST", url, 'request', data, 'response', obj);
                if (obj.errors && obj.errors.length) {                    
                    console.error("GraphQl call error: ", obj.errors[0]);
                }
                return obj;
            } catch (e) {
                console.error("failed to decode <" + text + ">");
                Api.showError('Internal Server Error<br/>We already investigate the issue');
                return text;
            }
        } catch (ex) {
            console.error("Failed to execute request to", url, data, ex);
            Api.showError('Internal Server Error<br/>We already investigate the issue');
            return { ok: false };
        }
    }

    static async postString(url = '', str) {
        try {
            const response = await fetch(url, {
                method: 'POST', // *GET, POST, PUT, DELETE, etc.
                headers: { 'Content-Type': 'application/json' },
                body: str
            });
            let text = await response.text();
            if (!response.ok) {
                if (response.status == 401) {
                    Api.showError('You do not have permission to perform this action');
                } else {
                    console.error("Failed to fetch", url, data, response);
                    Api.showError('Failed to process request<br/> Please refresh the page and try once more');
                }
            }
            try {
                let obj = JSON.parse(text);
                console.log("response", obj);
                return obj;
            } catch (e) {
                console.error("failed to decode <" + text + ">");
                Api.showError('Internal Server Error<br/>We already investigating the issue');
                return text;
            }
        } catch (ex) {
            console.error("Failed to execute request to", url, data);
            Api.showError('Internal Server Error<br/>We already investigating the issue');
            return { ok: false };
        }
    }
}
