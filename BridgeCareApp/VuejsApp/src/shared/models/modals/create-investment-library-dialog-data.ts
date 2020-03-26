import {InvestmentLibraryBudgetYear} from '@/shared/models/iAM/investment';
import {CriteriaDrivenBudgets} from '../iAM/criteria-driven-budgets';

export interface CreateInvestmentLibraryDialogData {
    showDialog: boolean;
    inflationRate: number;
    description: string;
    budgetOrder: string[];
    budgetYears: InvestmentLibraryBudgetYear[];
    budgetCriteria: CriteriaDrivenBudgets[];
}

export const emptyCreateInvestmentLibraryDialogData: CreateInvestmentLibraryDialogData = {
    showDialog: false,
    inflationRate: 0,
    description: '',
    budgetOrder: [],
    budgetYears: [],
    budgetCriteria: []
};
