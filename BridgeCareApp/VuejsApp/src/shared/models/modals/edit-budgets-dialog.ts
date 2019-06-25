export interface EditBudgetsDialogData {
    showDialog: boolean;
    budgets: string[];
    canOrderBudgets: boolean;
}

export const emptyEditBudgetsDialogData: EditBudgetsDialogData = {
    showDialog: false,
    budgets: [],
    canOrderBudgets: false
};

export interface EditBudgetsDialogGridData {
    name: string;
    index: number;
    previousName: string;
    isNew: boolean;
}

export interface EditedBudget {
    name: string;
    previousName: string;
    isNew: boolean;
}
