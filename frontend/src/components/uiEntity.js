import consts from "./consts";

export default class UiEntity {
    constructor(entity, sprite) {
        this.entity = entity;
        this.sprite = sprite;
    }

    update() {
        this.sprite.x = this.entity.pos.x * consts.PIXELS_IN_METER;
        this.sprite.y = this.entity.pos.y * consts.PIXELS_IN_METER;
    }
}