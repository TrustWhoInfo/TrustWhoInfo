const CSS = `
.twi-icon-prepend:before {
    content: ' ';
    width: 16px;
    height: 16px;
    display: inline-block;
    margin-right: 4px;
    position: relative;
    top: 2px;    
    background: url("data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAJgSURBVDhPvVNLaxNRFL73ziOXGE3aZBEli4D9AT4SjeZZjVBxqV0IXQQMuHTtwoVIQXDRboVIkbroUqFCVyahSVski0BqH7HF58KmQdI82kxn5o7njuPYWsGdH5yZM2fOd+6Zc75B/x0fPn+hK/W6x3pE2Lr/FW8rFY8giL5etxvQNDWl63rUMIyYKIqFK8PD13nOkQKr9feu7cZWRFPVpKZpWabrfowxIoKwRohQQMgISrLjaSIWfcnz7QJv8oU0Y/ojIIQMhERCSBVOG2KM0UGv99KFUKhSW1mlUHyaYKylksnbnEdMNoAQzEkadTpH3QODA9fS6bOCIExSSpc4mec0G1sZiD2Dc9fzxeIZk8cvHEBGcKLP6To+GwmHWlZYB9O48259XQTi1WQ8PnfM5ZqE3Hs8bhdgzLgjy46HnZ3WmBU6hHarNQKjeM798PlzLehks1qrBc0CpYWFlM70cjwWnYE2bvDYn+DDhI3cnC+VpsqLi1MGY6fbO+2AWUBV1btery/HfUzIq3yheIv7B6FqWl/dVzKiJGUIJhlFUcZEUWiRzY+fqNvtfu2g1AcrDPpPnlo64XHD9x5GKpF4gTC+3+10vna7nTVJkkYvRyLLYnO7Edjt9abBEAgFCTBMnTHYN5r5Sf0NEM9juHGzQS6Gwxuwgdze7h4C8aC+ovShm3Hr/T9hC2m+XB6BAkOyg85KktiEVp9ARxGMsJ8IZA4GOJ5MxDesdBtHpFytLbu+N5vZfaU/YYVMyJTm4B94kIjFvlkhE7YOfgF0EEMGm5BlGR00EEoWxJO10iwg9AOAXfk4S3Pt+QAAAABJRU5ErkJggg==");
}
`;

import Providers from './providers';
import Settings from './settings';

const provider = Providers.get(location.hostname);

function injectStylesheet() {
    const style = document.createElement("style");
    style.textContent = CSS;
    style.id = 'twi-css';
    document.head.appendChild(style);
}

function removeStylesheet() {
    const style = document.getElementById('twi-css');
    if (style) {
        style.remove();
    }
}

function install(){
    if (provider) {
        injectStylesheet();
        provider.install();
    }
}

function remove(){
    if (provider) {
        removeStylesheet();
        provider.uninstall();
    }
}

chrome.runtime.onMessage.addListener(function(request, sender, sendResponse)    {
    if(request.command === 'init'){
        install();
    } else {
        remove();
    }
    sendResponse({result: "success"});
});

function initialize() {
    chrome.storage.sync.get(['enabled', 'code'], function(data) {
        Settings.setEnabled(data.enabled);
        Settings.setCode(data.code);
        if(data.enabled){
            install();
        }else{
            remove();
        } 
    });
}

if (document.readyState != "loading") {
    initialize();
} else {
    window.addEventListener("DOMContentloaded", initialize);
}
