const chkEnabled = document.getElementById('chkEnabled');
const txtCode = document.getElementById('code');

function updateIcon(enabled) {
    chrome.tabs.query({active: true, currentWindow: true}, ([tab]) => {
        chrome.action.setIcon({tabId: tab.id, path: enabled ? 'images/icon64.png' : 'images/face64.png'});
    });    
}

chrome.storage.sync.get('enabled', function(data) {
    chkEnabled.checked = data.enabled;
    updateIcon(data.enabled);
});
chrome.storage.sync.get('code', function(data) {
    txtCode.value = data.code ?? "";
});
    
chkEnabled.onchange = (e) => {
    let value = this.checked;
    console.log("chkEnabled", value);
    chrome.storage.sync.set({enabled: value}, function() {
        console.log('`enabled` set to '+ value);
    });

    if (value) {
        chrome.tabs.query({active: true, currentWindow: true}, function(tabs) {
            chrome.tabs.sendMessage(tabs[0].id, {command: "init", hide: value}, function(response) {
                console.log('chrome.tabs.sendMessage', response);
            });
        });
    } else {
        chrome.tabs.query({active: true, currentWindow: true}, function(tabs) {
            chrome.tabs.sendMessage(tabs[0].id, {command: "remove", hide: value}, function(response) {
                console.log('chrome.tabs.sendMessage', response);
            });
        });
    }
    updateIcon(value);
};

txtCode.onblur = (e) => {
    const code = e.target.value;
    console.log("New code", code);
    chrome.storage.sync.set({code: code}, function() {
        console.log('`code` set to '+ code);
    });
};

