import Tass from './providers/tass.js';

const providers = {
    'tass.ru': () => new Tass(),
};

export default class Providers {
    static get(hostname) {
        const provider = providers[hostname];
        if (provider) {
            return provider();
        }
        return null;
    }
}