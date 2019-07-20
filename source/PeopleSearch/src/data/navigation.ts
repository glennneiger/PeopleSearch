import environment from 'environment';

export class Navigation {
    static Menu() {
        return [{
            text: 'People',
            icon: 'fa-exchange',
            show: true,
            enabled: true,
            subMenu: [
                {
                    text: 'Rabbit Search',
                    href: 'searchfast',
                    icon: 'fa-play',
                    selected: true,
                    status: null
                },
                {
                    text: 'Tortois Search',
                    href: 'searchslow',
                    icon: 'fa-play',
                    selected: false,
                    status: null
                },
                {
                    text: 'Add User',
                    href: 'create',
                    icon: 'fa-check-square',
                    selected: false,
                    status: 'completed'
                },
                {
                    text: 'Stats',
                    href: 'stats',
                    icon: 'fa-archive',
                    selected: false,
                    status: 'archived'
                }
            ]
        }]
    }
}
