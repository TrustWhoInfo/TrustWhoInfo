export default class Helper {
    /**
     * Used to bind all methods of given object so that
     * its this always refereneced to that object
     */
    static bind(obj) {
        const methods = Object.getOwnPropertyNames(Object.getPrototypeOf(obj));
        methods
            .filter(method => (method !== 'constructor'))
            .forEach((method) => { obj[method] = obj[method].bind(obj); });
    }

    static newId() {
        let array = [];
        for (let i = 0; i < 24; ++i) {
            let letter = Helper.alphabet[Math.trunc(Math.random() * Helper.alphabet.length)];
            array.push(letter);
        }
        return array.join('');
    }

    static isMobile() {
        return window.screen.width < 763;
    }    

    static isHorizontal() {
        return window.innerWidth > window.innerHeight;
    }

    static scrollToTop(node) {
        let scroll = Helper.getScrollParent(node);
        node.scrollIntoView();
        scroll.scrollTop -= 60;
    }

    static getScrollParent(node) {
        if (node == null) return null;      
        if (node.scrollHeight > node.clientHeight)
            return node;
        return Helper.getScrollParent(node.parentNode);
    }

    static shuffle(items) {
        var j, x, i;
        for (i = items.length - 1; i > 0; i--) {
            j = Math.floor(Math.random() * (i + 1));
            x = items[i];
            items[i] = items[j];
            items[j] = x;
        }
        return items;
    }

     static dateToString (value) {
        if (!value) return '';
        let date = new Date(value);
        let today = new Date();
        if (date.toLocaleDateString() == today.toLocaleDateString())
            return date.toLocaleTimeString();
        if (today - date < 86400 * 1000)
            return "Yesterday, " + date.toLocaleTimeString();
        return date.toLocaleDateString() + " " + date.toLocaleTimeString();
    }

    static getLocal(key) {
        let json = window.localStorage.getItem(key);
        if (!json) return null;
        try {
            return JSON.parse(json);
        } catch(e) {
            return null;
        }
    }

    static setLocal(key, obj) {
        try {
            const json = JSON.stringify(obj);
            window.localStorage.setItem(key, json);
        } catch(e) {
            log.error("Failed to store", key, "to local storage", obj, e);
        }
    }
}   

Helper.alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";