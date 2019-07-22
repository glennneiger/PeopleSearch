import { bindable, inject } from 'aurelia-framework';

@inject(Element)
export class BootstrapTooltipCustomAttribute {
    @bindable title: string;
    @bindable enabled: boolean = true;

    constructor(private element: HTMLElement) {
        this.element = element;
    }

    bind() {
        $(this.element).tooltip({
            title: this.title,
            delay: 750,
            trigger: 'hover'
        });

        if (!this.enabled) {
            $(this.element).tooltip('disable');
        }
    }

    enabledChanged(enabled: any): void {
        enabled ? $(this.element).tooltip('enable') : $(this.element).tooltip('disable');
    }
}
