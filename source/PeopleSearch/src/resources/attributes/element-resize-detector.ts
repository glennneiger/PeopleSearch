import * as erd from 'element-resize-detector';

export class ResizeableCustomAttribute {
    static inject = [Element];
    element: HTMLElement;
    callback;
    erd;

    constructor(element: HTMLElement) {
        this.element = element;
        this.erd = erd({ strategy: 'scroll' });
    }

    bind() {
        let element = this.element;

        let widthOld = element.offsetWidth;
        let heightOld = element.offsetHeight;

        this.callback = (x) => {
            let event = new CustomEvent("resize", {
                detail: {
                    width: this.element.offsetWidth,
                    height: this.element.offsetHeight,
                    widthOld: widthOld,
                    heightOld: heightOld
                }
            });

            this.element.dispatchEvent(event);

            widthOld = this.element.offsetWidth;
            heightOld = this.element.offsetHeight;
        };

        this.erd.listenTo(this.element, this.callback);
    }

    unbind() {
        if (this.callback) {
            this.erd.uninstall(this.element);
            this.callback = null;
        }
    }
}