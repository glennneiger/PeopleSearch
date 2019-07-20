export class SearchFilterValueConverter {
    toView(array, query, fields, showResults = false) {
        if (!query) {
            return showResults ? array : [];
        }

        let results = array.filter(item => {
            for ( let prop in item ) {
                if (item.hasOwnProperty(prop)) {
                    if (fields.indexOf(prop) != -1 && item[prop].toLowerCase().indexOf(query.toLowerCase()) != -1) {
                        return true;
                    }
                }
            };

            return false;
        });

        return results;
    }
}
