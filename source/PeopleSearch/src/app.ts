import { autoinject } from 'aurelia-framework';
import { activationStrategy, Redirect, Router } from 'aurelia-router';
import { DialogService } from 'aurelia-dialog';
import { EventAggregator, Subscription } from 'aurelia-event-aggregator';

@autoinject()
export class App {
    private user: any;
    private subscriptions: Subscription[] = [];
    correlationId: string;
    error: any;
    sidebarCollapsed: boolean;
    
    constructor(
        private eventAggregator: EventAggregator,
        private dialogService: DialogService,
        private router: Router
    ) {
        toastr.options.positionClass = 'toast-top-right';
        toastr.options.extendedTimeOut = 2000;
        toastr.options.timeOut = 5000;
    }

    async bind() {
    }

    attached() {
        
    }

    detached() {
        this.error = null;
        this.subscriptions.forEach(subscription => subscription.dispose());
    }

    public configureRouter(config, router) {
        this.router = router;;

        config.title = 'People Search';
        config.options.pushState = true;
        config.options.root = '/';
        
        config.map([
            {
                route: ['', 'index' ],
                name: 'Home',
                moduleId: 'people/index',
                nav:true
            },
            //{
            //    route: 'list',
            //    name: 'searchfast',
            //    moduleId: 'people/search'
            //},
            //{
            //    route: 'list',
            //    name: 'searchslow',
            //    moduleId: 'people/search'
            //},
            //{
            //    route: 'new',
            //    name: 'create',
            //    moduleId: 'people/new'
            //},
            //{
            //    route: 'stats',
            //    name: 'stats',
            //    moduleId: 'people/stats'
            //}
        ]);

        config.mapUnknownRoutes('errors/not-found');

        config.fallbackRoute('');
    }
}
