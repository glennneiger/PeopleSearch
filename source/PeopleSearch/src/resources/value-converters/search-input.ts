export class SearchInputHighlightValueConverter {
    toView(value, filter) {
        if (!filter) {
            return value;
        }

        let result = '';
        let position = 0;

        while (true) {
            let index = value.toLowerCase().indexOf(filter.toLowerCase(), position);

            if (index == -1) {
                result += value.substring(position);
                return result;
            }

            result += value.substring(position, index) + `<strong style="color: #00a4db;">${value.substr(index, filter.length)}</strong>`;

            position = index + filter.length;

            if (position >= value.length) return result;
        }
    }
}
