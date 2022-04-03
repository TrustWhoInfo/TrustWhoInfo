function updateIcon() {  
    chrome.storage.sync.get('enabled', function(data) {    
        chrome.tabs.query({active: true, currentWindow: true}, ([tab]) => {
            chrome.action.setIcon({tabId: tab.id, path: data.enabled ? 'images/icon64.png' : 'images/face64.png'});
        });    
    });    
}

chrome.runtime.onInstalled.addListener(function() {
    chrome.storage.sync.set({enabled: true}, function() {
      console.log("Hide image is on");
    });
  });

chrome.declarativeContent.onPageChanged.removeRules(undefined, function() {
    chrome.declarativeContent.onPageChanged.addRules([
        {
            conditions: [
                new chrome.declarativeContent.PageStateMatcher({pageUrl: {hostEquals: 'tass.ru'}}),
            ],
            actions: [
                new chrome.declarativeContent.ShowPageAction(),
            ]
        },
    ]);
    updateIcon();
});

updateIcon();

chrome.tabs.onActivated.addListener(function(e) {
    chrome.storage.sync.set({enabled: true}, function() {
      updateIcon();
    });
  });
