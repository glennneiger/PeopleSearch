import { autoinject, bindable, customElement } from 'aurelia-framework';

@customElement('header-nav')
@autoinject
export class HeaderNavCustomElement {
    @bindable sidebarCollapsed: boolean;
    @bindable user: any;

    constructor() {

    }
}