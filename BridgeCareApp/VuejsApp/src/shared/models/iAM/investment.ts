export interface InvestmentStrategyBudgetYear {
    investmentStrategyId: number;
    id: number;
    year: number;
    budgetName: string;
    budgetAmount: number;
}

export interface InvestmentStrategy {
    id: number;
    name: string;
    inflationRate: number;
    discountRate: number;
    description: string;
    budgetOrder: string[];
    budgetYears: InvestmentStrategyBudgetYear[];
    deletedBudgetYearIds: number[];
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

export const emptyInvestmentStrategy: InvestmentStrategy = {
    id: 0,
    name: '',
    inflationRate: 0,
    discountRate: 0,
    description: '',
    budgetOrder: [],
    budgetYears: [],
    deletedBudgetYearIds: []
};
