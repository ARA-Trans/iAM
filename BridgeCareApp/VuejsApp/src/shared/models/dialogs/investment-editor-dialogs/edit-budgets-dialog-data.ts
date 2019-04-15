export interface EditBudgetsDialogData {
    showDialog: boolean;
    budgets: string[];
}

export const emptyEditBudgetsDialogData: EditBudgetsDialogData = {
    showDialog: false,
    budgets: []
};
