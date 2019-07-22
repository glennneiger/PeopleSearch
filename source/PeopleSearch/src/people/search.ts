import { autoinject, observable } from 'aurelia-framework';
import { Router } from 'aurelia-router';
import { DialogService } from 'aurelia-dialog';
import { PersonComponentDialog } from 'people/resources/dialog/person-dialog';
import { PeopleService } from 'services/people-service';
import { Person } from 'models/person';


@autoinject
export class PeopleGrid {
    people: Person[] = [];
    peopleCount: number;
    componentsPage: any[];
    query: string;
    pageCount: number = 0;
    currentPage: number = 0;
    pageSizeOptions = [10, 25, 50, 100];
    pagerButtons: number[] = [];
    activated: boolean = false;
    @observable pageSize: number = 25;
    searchedValue: string;
    isProcessing: boolean = false;
    goSlow: boolean = false;

    constructor(
        private dialogService: DialogService,
        private peopleService: PeopleService,
        private router: Router
    )
    {}

    async activate(params): Promise<void> {
        let settings = JSON.parse(localStorage.getItem('settings')) || {};
        if (settings.pageSize) {
            this.pageSize = settings.pageSize;
        }
    }

    async pageSizeChanged(): Promise<void> {
        if (this.people.length > 0) {
            this.goToPage(0);
        
            let settings = JSON.parse(localStorage.getItem('settings')) || {};
            Object.assign(settings, { pageSize: this.pageSize });
            localStorage.setItem('settings', JSON.stringify(settings));
        }
    }

    async goToPage(pageIndex: number): Promise<void> {
        this.people.length = 0;
        this.currentPage = pageIndex;
        await this.search();
        this.updatePageCount();
    }

    async goSearch(): Promise<void> {
        this.currentPage = 0;
        if (this.query) {
            this.searchedValue = this.query;
            await this.search(); 
        }
        this.updatePageCount();
    }

    private updatePageCount(): void {
        this.pageCount = Math.ceil(this.peopleCount / this.pageSize);
        this.pagerButtons = Array.from({ length: Math.min(this.pageCount, 10) }, (v, k) => {
            if (this.pageCount - this.currentPage > 5) {
                return k + Math.max(0, this.currentPage - 5);
            } else {
                return k + Math.max(0, this.pageCount - 10);
            }
        });
    }
    view(person: Person, editMode: boolean): void {
        this.openDialog(person.id, editMode);
    }

    openEdit(): void {
        this.openDialog(0, true);
    }
    
    openDialog(personId: number, editMode: boolean): void {
        let dialogParams = Object.assign({}, { personId: personId, editMode: editMode, lock: true });
        this.dialogService.open({ viewModel: PersonComponentDialog, model: dialogParams });
    }

    async delete(person: Person): Promise<void> {
        await this.peopleService.delete(person.id);

        this.syncGrid(person);
    }

    private async search(): Promise<void> {
        this.isProcessing = true;
        var skip = this.currentPage * this.pageSize;
        let [pageResult, totalCount] = await Promise.all([this.peopleService.find(this.searchedValue, skip, this.pageSize, this.goSlow), this.peopleService.count(this.searchedValue)]);
        this.people = pageResult;
        this.peopleCount = totalCount;
        this.activated = true;
        this.isProcessing = false;
    }

    private syncGrid(person: Person): void {
        let index = this.people.findIndex(c => { return c.id === person.id });
        if (index > -1) this.people.splice(index, 1);
    }
}
