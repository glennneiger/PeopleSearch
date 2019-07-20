/**
 * @desc This class wraps our local storage manipulation into a service
 * and supports deep setting, getting, and removal of local storage values
*/

export class LocalStorageService {
    get(key: string): any {
        let path: string[] = key.split('.');
        key = path.shift();
        let value: Object = JSON.parse(localStorage.getItem(key)) || {};

        return this.getDeepValue(value, path);
    }

    set(key: string, newValue: Object): void {
        let path: string[] = key.split('.');
        key = path.shift();
        let value: Object = JSON.parse(localStorage.getItem(key)) || {};

        this.setDeepValue(value, path, newValue);
        localStorage.setItem(key, JSON.stringify(value));
    }

    remove(key: string): void {
        let path: string[] = key.split('.');
        key = path.shift();
        let value: Object = JSON.parse(localStorage.getItem(key));

        if (path.length > 0) {
            this.setDeepValue(value, path, null, true);
            localStorage.setItem(key, JSON.stringify(value));
        } else {
            localStorage.removeItem(key);
        }
    }

    private getDeepValue(value: Object, path: string[]): any {
        let key: string;

        while (value !== undefined && path.length > 0) {
            key = path.shift();
            value = value[key];
        }
        return value;
    }

    private setDeepValue(value: Object, path: string[], newValue: any, remove: boolean = false): void {
        let parent: Object = value;
        let property: string;

        while (path.length > 0) {
            property = path.shift();
            value = parent[property];

            if (value === undefined) {
                value = {};
                parent[property] = value;
            }

            if (path.length > 0) {
                parent = value;
            }
        }

        remove ? delete parent[property] : parent[property] = newValue;
    }
}
