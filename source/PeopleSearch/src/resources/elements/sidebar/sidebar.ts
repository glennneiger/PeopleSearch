import { autoinject, bindable, bindingMode, customElement } from 'aurelia-framework';
import { LocalStorageService } from 'services/local-storage-service';
import { Navigation } from 'data/navigation';
import { Router } from 'aurelia-router';

@customElement('sidebar-nav')
@autoinject
export class SidebarNavCustomElement {
    @bindable({ defaultBindingMode: bindingMode.twoWay }) sidebarCollapsed: boolean = true;;
    @bindable user;
    navigation: any;
    navigationCollapsed: boolean;
    navigationLoaded: boolean = false;

    constructor(
        private localStorageService: LocalStorageService,
        private router: Router,
        private element: Element
    ) {

    }

    attached() {
        this.element.classList.add('sidebar-transition');
        this.navigation = Navigation.Menu();
        let route = this.router.currentInstruction;
        this.navigation.forEach(item => {
            item.show = false;

            if (item.href === route.config.name) {
                item.selected = true;
                item.show = true;
            } else {
                item.selected = false;
            }

        });

        let sidebarCollapsed = this.localStorageService.get('sidebar');
        if (sidebarCollapsed.collapsed) {
            this.sidebarCollapsed = true;
            this.navigationCollapsed = true;
        } else {
            this.sidebarCollapsed = false;
            this.navigationCollapsed = false;
        }

        this.navigationLoaded = true;
    };

    /**
     * Expands or collapses the sidebar
     */
    toggleSidebar(): void {
        this.sidebarCollapsed = !this.sidebarCollapsed;

        if (!this.navigationCollapsed) {
            this.navigationCollapsed = true;
        } else {
            setTimeout(() => {
                this.navigationCollapsed = false;
            }, 400);
        }

        this.localStorageService.set('sidebar.collapsed', this.sidebarCollapsed);
    }

    /**
     * Navigates to route
     * @param config 
     */
    goToRoute(config: any): void {
        let current = document.getElementsByClassName('selected');
        if (current.length) {
            current[0].classList.remove('selected');
        }

        config.element.classList.add('selected');

        this.router.navigateToRoute(config.href);
    }
}