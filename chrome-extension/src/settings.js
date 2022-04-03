export default class Settings {
    static setEnabled(value) {
        Settings.enabled = value;
    }

    static setCode(value) {
        Settings.code = value;
    }
}

Settings.enabled = true;
Settings.code = "";