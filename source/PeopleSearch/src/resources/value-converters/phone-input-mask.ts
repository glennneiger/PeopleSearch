import { inject, customAttribute, DOM } from 'aurelia-framework';
import * as Inputmask from "inputmask";

@customAttribute('phone-input-mask')
@inject(DOM.Element)
export class PhoneInputMask {
    private element: HTMLInputElement;

    constructor(element: HTMLInputElement) {
        this.element = element;
    }

    attached() {
        try {
            let im = new Inputmask("(999) 999-9999");
            im.mask(this.element);
        }
        catch (error) {

            var message = error;
        }
    }
}