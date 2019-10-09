import { CriteriaDrivenBudgets } from './criteria-driven-budgets';

export interface InvestmentLibraryBudgetYear {
    id: number | string;
    year: number;
    budgetName: string;
    budgetAmount: number;
}

export interface InvestmentLibrary {
    id: number | string;
    name: string;
    inflationRate: number;
    description: string;
    budgetOrder: string[];
    budgetYears: InvestmentLibraryBudgetYear[];
    budgetCriteria: CriteriaDrivenBudgets[];
}

export interface BudgetYearsGridData {
    year: number;
    [budgetName: string]: number;
}

export const emptyInvestmentLibrary: InvestmentLibrary = {
    id: 0,
    name: '',
    inflationRate: 0,
    description: '',
    budgetOrder: [],
    budgetYears: [],
    budgetCriteria: []
};
