export class NumberFormatValueConverter {
    toView(value) {
        if (value === null || value === undefined) {
            return '--';
        }

        return Number.prototype.toLocaleString.call(value);
    }
}
