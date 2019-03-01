export interface InvestmentStrategy {
    id: number;
    name: string;
}

export interface InvestmentStrategyDetail {
    investmentStrategyId: number;
    inflationRate: number;
    discountRate: number;
    year: number;
    budgets: InvestmentStrategyBudget[];
    description: string;
}

export interface InvestmentStrategyBudget {
    name: string;
    amount: number;
}
