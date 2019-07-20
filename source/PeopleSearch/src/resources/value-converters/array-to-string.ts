import { autoinject } from 'aurelia-dependency-injection';

@autoinject
export class ArrayToStringValueConverter {
    toView(value) {
        if (Array.isArray(value)) {
            return value.join(', ');    
        } else {
            return value;
        }
    }
}
