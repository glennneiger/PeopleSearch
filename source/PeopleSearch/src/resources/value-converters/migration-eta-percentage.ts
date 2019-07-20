export class MigrationEtaPercentageValueConverter {
    toView(value) {
        let plannedStartDate: any = new Date(value.plannedStartDate);
        let estimatedCompletionTime: any = new Date(value.estimatedCompletionTime);
        let today: any = new Date();

        let timeBetweenStartAndEnd: number = (estimatedCompletionTime - plannedStartDate);
        let timeBetweenStartAndToday: number = (today - plannedStartDate);

        let migrationEtaPercentage: number = Math.round(timeBetweenStartAndEnd / timeBetweenStartAndEnd * 100);

        return migrationEtaPercentage;
    }
}
