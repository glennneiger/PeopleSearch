export class PluralValueConverter {
    toView(value) {
        if (['ch', 'sh'].indexOf(value.charAt(value.length - 2)) !== - 1 || ['s', 'x', 'z'].indexOf(value.charAt(value.length - 1)) !== -1) {
            return value + 'es';
        } else {
            return value + 's';
        }
    }
}
