import { autoinject } from 'aurelia-framework';
import { EventAggregator } from 'aurelia-event-aggregator';
import { HttpRequestMessage, HttpResponseMessage } from 'aurelia-http-client';

@autoinject()
export class HttpInterceptor {

    constructor(private eventAggregator: EventAggregator) {}

    async request(request: HttpRequestMessage): Promise<HttpRequestMessage> {
        let value = request.headers.get("anonymous");
        if (value !== "true") {
            console.log("Anonymous request in process");
        }
        return request;
    }

    async requestError(error: HttpRequestMessage): Promise<HttpRequestMessage> {
        throw error;
    }

    async response(response: HttpResponseMessage): Promise<HttpResponseMessage> {
        return response;
    }

    async responseError(error: HttpResponseMessage): Promise<void> {
        if (error.statusCode >= 400 && error.statusCode <= 500) this.eventAggregator.publish("error", error);
    }
}
