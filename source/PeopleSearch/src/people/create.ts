import { autoinject, bindable } from 'aurelia-framework';
import { Router } from 'aurelia-router';
import { PersonComponentDialog } from 'people/resources/dialog/person-dialog';
import { Person } from 'models/person';

@autoinject()
export class Index {
    person: Person;

    constructor(
        private router: Router
    ) {
    }

    close(): void{
    }
}
