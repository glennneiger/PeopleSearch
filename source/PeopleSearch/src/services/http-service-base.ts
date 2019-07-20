import { autoinject, inject } from 'aurelia-framework';
import { NewInstance } from 'aurelia-dependency-injection';
import { HttpClient } from 'aurelia-http-client';
import environment from 'environment';

@autoinject()
export class HttpServiceBase {   
    rootUrl: string;

    constructor(
        protected  httpClient: HttpClient) {
        this.rootUrl = environment.apiUrl;
    }
}
