import * as numeral from 'numeral'

export class NumberFormatValueConverter {
    toView(value: number, format: string) {
        if (value === null || value === undefined) {
            return '--';
        }
        var string = numeral(value).format(format);
        return string;
    }
}
