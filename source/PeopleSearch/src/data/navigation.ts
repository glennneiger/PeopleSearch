import environment from 'environment';

export class Navigation {
    static Menu() {
        return [
            {
                text: 'Home',
                href: 'people',
                icon: 'fa-home',
                selected: true,
                status: null
            },
            {
                text: 'People Search',
                href: 'search',
                icon: 'fa-search',
                selected: false,
                status: null
            },
            {
                text: 'Add Person',
                href: 'create',
                icon: 'fa-plus',
                selected: false,
                status: null
            }
        ]
    }
}
