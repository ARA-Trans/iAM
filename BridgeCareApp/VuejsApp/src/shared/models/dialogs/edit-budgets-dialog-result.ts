export interface BudgetNames {
    name: string;
    previousName: string;
}

export interface EditBudgetsDialogResult {
    canceled: boolean;
    budgets: BudgetNames[];
}
