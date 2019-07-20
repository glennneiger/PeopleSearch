import { Router } from 'aurelia-router';
import { inject } from 'aurelia-framework';

@inject(Router)
export class Index {
    message = 'Create, Edit or Search People';

  constructor(
      private router: Router) { }

  activate() {
        
    }
}
