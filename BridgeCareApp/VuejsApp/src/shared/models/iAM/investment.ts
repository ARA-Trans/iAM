export interface InvestmentStrategy {
    networkId: number;
    simulationId: number;
    name: string;
    inflationRate: number;
    discountRate: number;
    budgetYears: InvestmentStrategyBudgetYear[];
    budgetOrder: string[]
    description: string;
}

export interface InvestmentStrategyBudgetYear {
    year: number;
    budgets: InvestmentStrategyBudget[];
}

export interface InvestmentStrategyBudget {
    name: string;
    amount: number;
}

export interface SavedInvestmentStrategy {
    networkId: number;
    simulationId: number;
    name: string;
    inflationRate: number;
    discountRate: number;
    budgetYears: InvestmentStrategyBudgetYear[];
    budgetOrder: string[]
    description: string;
    deletedBudgetYears: number[];
    deletedBudgets: string[];
}

export interface InvestmentStrategyGridData {
    year: number;
    [budgetName: string]: number;
}

export interface EditBudgetsDialogGridData {
    name: string;
    index: number;
}

export const emptyInvestmentStrategy: InvestmentStrategy = {
    networkId: 0,
    simulationId: 0,
    name: '',
    inflationRate: 0,
    discountRate: 0,
    budgetYears: [],
    budgetOrder: [],
    description: '',
};
