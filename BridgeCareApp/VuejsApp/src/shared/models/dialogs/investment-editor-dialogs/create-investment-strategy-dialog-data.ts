import {InvestmentStrategyBudgetYear} from '@/shared/models/iAM/investment';

export interface CreateInvestmentStrategyDialogData {
    showDialog: boolean;
    inflationRate: number;
    discountRate: number;
    description: string;
    budgetOrder: string[];
    budgetYears: InvestmentStrategyBudgetYear[];
}

export const emptyCreateInvestmentStrategyDialogData: CreateInvestmentStrategyDialogData = {
    showDialog: false,
    inflationRate: 0,
    discountRate: 0,
    description: '',
    budgetOrder: [],
    budgetYears: []
};
