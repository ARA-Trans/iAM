import {CriteriaDrivenBudgets} from '@/shared/models/iAM/investment';

export interface EditBudgetsDialogData {
    showDialog: boolean;
    budgets: string[];
    canOrderBudgets: boolean;
    criteriaBudgets: CriteriaDrivenBudgets[];
    scenarioId: number;
}

export const emptyEditBudgetsDialogData: EditBudgetsDialogData = {
    showDialog: false,
    budgets: [],
    canOrderBudgets: false,
    criteriaBudgets: [{budgetName: '', criteria: '', _id: '', scenarioId: 0}],
    scenarioId: 0
};

export interface EditBudgetsDialogGridData {
    name: string;
    index: number;
    previousName: string;
    isNew: boolean;
    criteriaBudgets: CriteriaDrivenBudgets;
}

export interface EditedBudget {
    name: string;
    previousName: string;
    isNew: boolean;
    criteriaBudgets: CriteriaDrivenBudgets;
}
