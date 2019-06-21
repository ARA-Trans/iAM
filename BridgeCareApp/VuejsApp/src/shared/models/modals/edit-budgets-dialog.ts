export interface EditBudgetsDialogData {
    showDialog: boolean;
    budgets: string[];
}

export const emptyEditBudgetsDialogData: EditBudgetsDialogData = {
    showDialog: false,
    budgets: []
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
