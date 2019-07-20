import { Aurelia, LogManager } from 'aurelia-framework';
import { HttpClient } from 'aurelia-http-client';
import { DialogSettings } from 'aurelia-dialog';
import { HttpInterceptor } from 'services/http-interceptor';
import { ConsoleAppender } from 'aurelia-logging-console';
import { library } from '@fortawesome/fontawesome-svg-core';
import { faAngleDown, faAngleRight, faCopy, faFlask, faFolderOpen, faInfoCircle, faPenAlt, faPlus, faSave, faTrash, faWater } from '@fortawesome/free-solid-svg-icons';
import environment from './environment';

import * as bootstrap from 'bootstrap';
window['bootstrap'] = bootstrap;
import * as moment from 'moment';
window['moment'] = moment;
import * as toastr from 'toastr';
window['toastr'] = toastr;

library.add(
    faAngleDown, faAngleRight, faCopy, faFlask, faFolderOpen, faInfoCircle, faPenAlt, faPlus, faSave, faTrash, faWater
)

LogManager.addAppender(new ConsoleAppender());
LogManager.setLevel(LogManager.logLevel.warn);

export function configure(aurelia: Aurelia) {
    aurelia.use
        .standardConfiguration()
        .feature('resources')
        .plugin('aurelia-validation')
        .plugin('aurelia-animator-css')
        .plugin('aurelia-dialog', (config: DialogSettings) => {
            config.settings.keyboard = true;
            config.settings.overlayDismiss = true;
        })
        .plugin('ag-grid-aurelia')
        .plugin('aurelia-fontawesome');

    const httpInterceptor = aurelia.container.get(HttpInterceptor);
    const httpClient = aurelia.container.get(HttpClient);

    httpClient.configure((config: { withInterceptor: (arg0: any) => void; }) => {
        config.withInterceptor(httpInterceptor);
    });

    aurelia.use.instance(HttpClient, httpClient);

    if (environment.debug) {
        aurelia.use.developmentLogging();
    }

    if (environment.testing) {
        aurelia.use.plugin('aurelia-testing');
    }

    aurelia.start().then(() => aurelia.setRoot());
}
