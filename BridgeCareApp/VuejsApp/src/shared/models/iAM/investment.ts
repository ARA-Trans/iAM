export interface InvestmentLibraryBudgetYear {
    investmentLibraryId: number;
    id: number;
    year: number;
    budgetName: string;
    budgetAmount: number;
}

export interface InvestmentLibrary {
    id: number;
    name: string;
    inflationRate: number;
    discountRate: number;
    description: string;
    budgetOrder: string[];
    budgetYears: InvestmentLibraryBudgetYear[];
}

export interface BudgetYearsGridData {
    year: number;
    [budgetName: string]: number;
}

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

export const emptyInvestmentLibrary: InvestmentLibrary = {
    id: 0,
    name: '',
    inflationRate: 0,
    discountRate: 0,
    description: '',
    budgetOrder: [],
    budgetYears: []
};
