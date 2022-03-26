/*global globalThis*/

export default class Api {
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
                return JSON.parse(text);
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

    static showError(errorMessage) {
        const Toastify = globalThis.Toastify;
        if (!Toastify) return
        Toastify({
            text: errorMessage,
            duration: 3000,
            newWindow: true,
            close: true,
            gravity: "top",
            position: 'right',
            backgroundColor: "linear-gradient(to right, rgb(176, 0, 146), rgb(97, 10, 103))",
            stopOnFocus: true,
        }).showToast();
    }

    static showWarning(errorMessage) {
        const Toastify = globalThis.Toastify;
        if (!Toastify) return
        Toastify({
            text: errorMessage,
            duration: 3000,
            newWindow: true,
            close: true,
            gravity: "top",
            position: 'center',
            backgroundColor: "linear-gradient(to right, rgb(222 144 0), rgb(255 118 0))",
            stopOnFocus: true,
        }).showToast();
    }

    static generateCode(users) {
        console.log("Generating code", users);
        const request = {
            Entities: users.map(u=>{return {
                Id: u.name,
                Level: u.level,
                Levels: u.levels,
            };}),
        };
        return this.postData('/api/generateCode', request);
    }
} 