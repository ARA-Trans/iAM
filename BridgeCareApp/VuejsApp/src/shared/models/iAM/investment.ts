export interface InvestmentLibraryBudgetYear {
    id: string;
    year: number;
    budgetName: string;
    budgetAmount: number | null;
    criteriaDrivenBudgetId: string;
}

export interface CriteriaDrivenBudget {
    id: string;
    budgetName: string;
    criteria: string;
}

export interface InvestmentLibrary {
    id: string;
    name: string;
    owner?: string;
    shared?: boolean;
    inflationRate: number;
    description: string;
    budgetOrder: string[];
    budgetYears: InvestmentLibraryBudgetYear[];
    criteriaDrivenBudgets: CriteriaDrivenBudget[];
}

export interface BudgetYearsGridData {
    year: number;
    [budgetName: string]: number | null;
}

export const emptyInvestmentLibrary: InvestmentLibrary = {
    id: '0',
    name: '',
    inflationRate: 0,
    description: '',
    budgetOrder: [],
    budgetYears: [],
    criteriaDrivenBudgets: []
};

export const emptyCriteriaDrivenBudget: CriteriaDrivenBudget = {
    id: '0',
    budgetName: '',
    criteria: ''
};
