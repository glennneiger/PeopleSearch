import { autoinject, bindable, bindingMode, observable, BindingEngine, customElement } from 'aurelia-framework';

@customElement('search-input')
@autoinject
export class SearchInputCustomElement {
    @bindable data;
    @bindable({ defaultBindingMode: bindingMode.twoWay }) value;
    @bindable key = 'name';
    @bindable id = '';
    @bindable customEntry = false;
    @bindable resultsLimit = null;
    @bindable debounce = 0;
    @bindable onSelect;
    @bindable instantCleanEmpty = true;
    @bindable disabled = false;
    @bindable openOnFocus = false;
    @bindable focusFirst = true;
    @bindable selectSingleResult = false;
    @bindable autocomplete = false;
    @bindable loadingText = 'Loading...';
    @bindable inputClass = '';
    @bindable placeholder = '';
    @bindable noResultsText = 'No Results';

    promiseQueue = [];
    dropdown;
    dropdownMenu;
    input;
    displayData = [];
    @observable filter = '';
    focusedIndex = -1;
    focusedItem = null;
    loading = false;

    dataObserver: any;
    openListener: any;
    outsideClickListener: any;
    keyDownListener: any;
    ignoreChange: boolean;

    constructor(private bindingEngine: BindingEngine) {
        this.bindingEngine = bindingEngine;
        this.openListener = () => this.openDropdown();
        this.outsideClickListener = event => this.handleBlur(event);
        this.keyDownListener = event => this.onKeyDown(event);
    }

    bind() {
        if (Array.isArray(this.data)) {
            this.dataObserver = this.bindingEngine.collectionObserver(this.data).subscribe(() => {
                this.checkCustomEntry();
                this.applyPlugins();
            });
        }

        this.checkCustomEntry();
    }

    attached() {
        this.dropdownMenu = this.dropdown.getElementsByClassName("dropdown-menu")[0];

        if (this.openOnFocus) {
            this.input.addEventListener('focus', this.openListener);
            this.input.addEventListener('click', this.openListener);
        }

        document.addEventListener('click', this.outsideClickListener);
        this.input.addEventListener('keydown', this.keyDownListener);
    }

    detached() {
        if (this.dataObserver) {
            this.dataObserver.dispose();
        }

        document.removeEventListener('click', this.outsideClickListener);
        this.input.removeEventListener('keydown', this.keyDownListener);

        if (this.openOnFocus) {
            this.input.removeEventListener('focus', this.openListener);
            this.input.removeEventListener('click', this.openListener);
        }
    }

    dataChanged() {
        if (this.dataObserver) {
            this.dataObserver.dispose();
        }

        if (Array.isArray(this.data)) {
            this.dataObserver = this.bindingEngine.collectionObserver(this.data).subscribe(() => {
                this.checkCustomEntry();
                this.applyPlugins();
            });
        }
    }

    valueChanged() {
        let newFilter = this.getName(this.value);

        if (newFilter !== this.filter) {
            this.ignoreChange = true;
            this.filter = newFilter;
        }
    }

    openDropdown() {
        if (this.dropdownMenu.classList.contains('show')) {
            return;
        }

        this.dropdownMenu.classList.add('show');
        this.focusNone();
        this.applyPlugins();
    }

    doFocusFirst() {
        if (this.focusFirst && this.displayData.length > 0) {
            this.focusedIndex = 0;
            this.focusedItem = this.displayData[0];
        }
    }

    checkCustomEntry() {
        if (this.data.length > 0 && typeof this.data[0] !== 'string') {
            this.customEntry = false;
        }
    }

    filterChanged() {
        if (this.ignoreChange) {
            this.ignoreChange = false;
            return;
        }

        this.applyPlugins()
            .then(() => {
                if (this.instantCleanEmpty && this.filter.length === 0) {
                    this.value = null;

                    if (typeof this.onSelect === 'function') {
                        this.onSelect({ item: null });
                    }
                } else if (this.customEntry) {
                    this.value = this.filter;

                    if (typeof this.onSelect === 'function') {
                        this.onSelect({ item: this.value });
                    }
                } else if (this.selectSingleResult && this.displayData.length === 1) {
                    this.itemSelected(this.displayData[0]);
                }
            });
    }

    applyPlugins() {
        this.focusNone();
        let localData;

        if (typeof this.data === 'function') {
            this.displayData = [];
            this.loading = true;

            let promise = this.data({ filter: this.filter, limit: this.resultsLimit })
                .then(data => {
                    if (this.promiseQueue.length > 1) {
                        this.promiseQueue.splice(0, 1);
                        return;
                    }

                    this.displayData = data;
                    this.doFocusFirst();
                    this.promiseQueue.splice(0, 1);
                    this.loading = false;
                })
                .catch(error => {
                    this.loading = false;
                    this.displayData = [];
                    throw error;
                });

            this.promiseQueue.push(promise);
            return promise;
        }

        localData = [].concat(this.data);
        if (this.filter && this.filter.length > 0) {
            localData = this.doFilter(localData);
        }

        if (!this.isNull(this.resultsLimit) && !isNaN(this.resultsLimit)) {
            localData = localData.slice(0, this.resultsLimit);
        }

        this.displayData = localData;
        this.doFocusFirst();

        return Promise.resolve({});
    }

    focusNone() {
        this.focusedItem = null;
        this.focusedIndex = -1;
    }

    doFilter(toFilter) {
        return toFilter.filter(item => !this.isNull(item) && this.getName(item).toLowerCase().indexOf(this.filter.toLowerCase()) > -1);
    }

    getName(item) {
        if (this.isNull(item)) {
            return '';
        }

        if (typeof item === 'object' && item.hasOwnProperty(this.key)) {
            return item[this.key].toString();
        }

        return item.toString();
    }

    resetFilter() {
        if (this.filter.length === 0) {
            this.value = null;
        }

        let newFilter;
        if (this.isNull(this.value)) {
            newFilter = '';
        } else {
            newFilter = this.getName(this.value);
        }

        if (newFilter !== this.filter) {
            this.ignoreChange = true;
            this.filter = newFilter;
        }
    }


    handleBlur(event) {
        if (!this.dropdownMenu.classList.contains('show')) {
            return;
        }

        setTimeout(() => {
            if (!this.dropdownMenu.contains(event.target)) {
                this.dropdownMenu.classList.remove('show');
                this.focusNone();
                this.resetFilter();
            }
        }, this.debounce);
    }

    itemSelected(item) {
        this.value = item;
        this.dropdownMenu.classList.remove('show');

        let newFilter = this.getName(this.value);
        if (newFilter !== this.filter) {
            this.ignoreChange = true;
            this.filter = newFilter;
        }

        if (typeof this.onSelect === 'function') {
            this.onSelect({ item: item });
        }
    }

    isNull(item) {
        return item === null || item === undefined;
    }

    onKeyDown(event) {
        if (this.dropdownMenu.classList.contains('show')) {
            this.switchKeyCode(event.keyCode);
            return;
        }

        this.applyPlugins()
            .then(() => {
                this.switchKeyCode(event.keyCode);
                this.dropdownMenu.classList.add('show');
            });
    }

    switchKeyCode(keyCode) {
        switch (keyCode) {
            case 40:
                return this.handleDownKey();
            case 38:
                return this.handleUpKey();
            case 13:
            case 9:
                return this.handleEnter();
            case 27:
                return this.handleEscape();
            default:
                return;
        }
    }

    handleDownKey() {
        if (this.focusedIndex >= this.displayData.length - 1) {
            return;
        }

        this.focusedIndex++;
        this.focusedItem = this.displayData[this.focusedIndex];
    }

    handleUpKey() {
        if (this.focusedIndex === 0) {
            return;
        }

        this.focusedIndex--;
        this.focusedItem = this.displayData[this.focusedIndex];
    }

    handleEnter() {
        if (this.displayData.length === 0 || this.focusedIndex < 0 || !this.dropdownMenu.classList.contains('show')) {
            return;
        }

        this.itemSelected(this.displayData[this.focusedIndex]);
    }

    handleEscape() {
        this.dropdownMenu.classList.remove('show');
        this.focusNone();
        this.resetFilter();
    }
}
