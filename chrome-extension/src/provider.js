import Api from "./api";
import Settings from "./settings";

export default class Provider {
    install() {

    }

    uninstall() {

    }

    async getTrustLevels(news) {
        return await Api.getTrustLevels(Settings.code, news);
    }
    
    levelColor(level) {
        const normalized = (level + 100) / 200; // 0..1      
        const redHue = 0;
        const greenHue = 114;
        const hue = redHue + normalized * (greenHue - redHue);
        return `hsl(${hue}deg, 100%, 50%)`;
    }
    
    createPreviewIndicator(level) {
        const ui = document.createElement("div");
        ui.classList.add("twi-icon-prepend");
        ui.style.marginTop = '4px';
        ui.textContent = "Trust level: " + (level > 0 ? `+${level}` : level < 0 ? `${level}` : '0');
        ui.style.color = this.levelColor(level);
        ui.style.textShadow = "1px 1px 1px #888";
        return ui;
    }
}