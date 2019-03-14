import {InvestmentStrategy} from '@/shared/models/iAM/investment';

export interface CreateInvestmentStrategyDialogResult {
    canceled: boolean;
    newInvestmentStrategy: InvestmentStrategy;
}
