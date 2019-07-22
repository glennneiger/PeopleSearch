import { autoinject, customElement, bindable } from 'aurelia-framework';
import { ValidationRules, ValidationController, validateTrigger } from 'aurelia-validation';
import { DialogController } from 'aurelia-dialog';
import { Router } from 'aurelia-router';
import { Person } from 'models/person';
import { PeopleService } from 'services/people-service';

@customElement('person-dialog')
@autoinject
export class PersonComponentDialog {
    dialog: any = {};
    @bindable person: Person;
    editMode: boolean = false;
    isDialog: boolean = false;
    firstNameRules: any;
    lastNameRules: any;
    phoneNumberRules: any;
    @bindable isProcessing: boolean = false;
    initialized: boolean = false;
    constructor(
        public dialogController: DialogController,
        private validationController: ValidationController,
        private router: Router,
        private peopleService: PeopleService
    ) {
        this.validationController.validateTrigger = validateTrigger.changeOrBlur;
    }

    async bind() {
        if (!this.initialized) {
            await this.initializeAsync(0, false, true);
        }
    }

    async activate(model: any) {
        if (!this.initialized) {
            await this.initializeAsync(model.personId, true, model.editMode);
        }
    }

    async initializeAsync(personId: number, isDialog: boolean, editMode: boolean): Promise<void> {
        this.isDialog = isDialog;
        this.editMode = editMode;
        if (personId) {
            this.isProcessing = true;
            this.person = await this.peopleService.getById(personId);
            this.isProcessing = false;
        } else {
            this.person = Object.assign(new Person(), { id: 0, firstName: null, lastName: null, phoneNumber: null });
        }
        this.initializeValidation();
        this.initialized = true;
    }

    initializeValidation(): void {
        this.firstNameRules = ValidationRules
            .ensure('firstName')
            .satisfies(val => val && val.length > 2 && val.length < 51, config => config.useDebounceTimeout(300))
            .withMessage('Please enter a first name between 3 and 50 characters.')
            .on(this.person).rules;

        this.lastNameRules = ValidationRules
            .ensure('lastName')
            .satisfies(val => val && val.length > 2 && val.length < 51, config => config.useDebounceTimeout(300))
            .withMessage('Please enter a last name between 3 and 50 characters.')
            .on(this.person).rules;

        this.phoneNumberRules = ValidationRules
            .ensure('phoneNumber')
            .satisfies(val => val && val.replace(/[^0-9]/g, '').length === 10, config => config.useDebounceTimeout(300))
            .withMessage('Please enter a valid phone number (555) 555-5555.')
            .on(this.person).rules;
    }

    async edit(): Promise<void> {
        this.editMode = true;
    }

    async save(): Promise<void> {
        this.validationController.validate()
            .then(async result => {
                if (result.valid) {
                    this.isProcessing = true;
                    if (this.person.id === 0) {
                        this.person = await this.peopleService.create(this.person);
                    } else {
                        await this.peopleService.save(this.person);
                    }
                    this.isProcessing = false;
                    if (this.isDialog) {
                        this.dialogController.ok(this.dialog);
                    } else {
                        this.router.navigateToRoute('people');
                    }
                }
            });
    }

}