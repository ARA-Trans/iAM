import {InvestmentLibraryBudgetYear} from '@/shared/models/iAM/investment';

export interface CreateInvestmentLibraryDialogData {
    showDialog: boolean;
    inflationRate: number;
    discountRate: number;
    description: string;
    budgetOrder: string[];
    budgetYears: InvestmentLibraryBudgetYear[];
}

export const emptyCreateInvestmentLibraryDialogData: CreateInvestmentLibraryDialogData = {
    showDialog: false,
    inflationRate: 0,
    discountRate: 0,
    description: '',
    budgetOrder: [],
    budgetYears: []
};
