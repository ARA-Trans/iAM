import {CriteriaDrivenBudget} from '@/shared/models/iAM/investment';

export interface EditBudgetsDialogData {
    showDialog: boolean;
    criteriaDrivenBudgets: CriteriaDrivenBudget[];
    scenarioId: number;
}

export const emptyEditBudgetsDialogData: EditBudgetsDialogData = {
    showDialog: false,
    criteriaDrivenBudgets: [],
    scenarioId: 0
};
