import {InvestmentLibraryBudgetYear, CriteriaDrivenBudget} from '@/shared/models/iAM/investment';

export interface CreateInvestmentLibraryDialogData {
    showDialog: boolean;
    inflationRate: number;
    description: string;
    budgetOrder: string[];
    budgetYears: InvestmentLibraryBudgetYear[];
    budgetCriteria: CriteriaDrivenBudget[];
}

export const emptyCreateInvestmentLibraryDialogData: CreateInvestmentLibraryDialogData = {
    showDialog: false,
    inflationRate: 0,
    description: '',
    budgetOrder: [],
    budgetYears: [],
    budgetCriteria: []
};
