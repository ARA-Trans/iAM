export interface InvestmentStrategySelectOption {
    id: number;
    name: string;
}

export interface InvestmentStrategy {
    id: number;
    name: string;
    inflationRate: number;
    discountRate: number;
    budgetYears: InvestmentStrategyBudgetYear[];
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

export const emptyInvestmentStrategy: InvestmentStrategy = {
    id: 0,
    name: '',
    inflationRate: 0,
    discountRate: 0,
    budgetYears: [],
    description: '',
};
