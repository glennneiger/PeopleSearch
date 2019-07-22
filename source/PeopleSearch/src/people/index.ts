import { autoinject, Lazy, LogManager, bindable } from 'aurelia-framework';
import { Router } from 'aurelia-router';
import { PeopleService } from 'services/people-service';
import { Stats, NameStats } from 'models/stats';

@autoinject()
export class Index {
    isLoaded: boolean = false;
    stats: Stats;

    constructor(
        private router: Router,
        private peopleService: PeopleService
    ) { }

    async activate() {
        this.stats = await this.peopleService.getStats();
        this.isLoaded = true;
    }
}
