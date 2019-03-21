import axios from 'axios';
import {InvestmentStrategy, SavedInvestmentStrategy} from '@/shared/models/iAM/investment';
import {mockInvestmentStrategies} from '@/shared/utils/mock-data';

axios.defaults.baseURL = process.env.VUE_APP_URL;

export default class InvestmentEditorService {
    /**
     * Gets all investment strategies a user can read/edit
     */
    getInvestmentStrategies(): Promise<InvestmentStrategy[]> {
        return Promise.resolve<InvestmentStrategy[]>(mockInvestmentStrategies);
        // TODO: add axios web service call for investment strategies
    }

    /**
     * Creates/updates an investment strategy
     * @param savedInvestmentStrategy The investment strategy for creating/updating
     */
    saveInvestmentStrategyToLibrary(savedInvestmentStrategy: SavedInvestmentStrategy): Promise<InvestmentStrategy> {
        return Promise.resolve<InvestmentStrategy>({
            networkId: savedInvestmentStrategy.networkId,
            simulationId: savedInvestmentStrategy.simulationId,
            name: savedInvestmentStrategy.name,
            inflationRate: savedInvestmentStrategy.inflationRate,
            discountRate: savedInvestmentStrategy.discountRate,
            budgetYears: savedInvestmentStrategy.budgetYears,
            budgetOrder: savedInvestmentStrategy.budgetOrder,
            description: savedInvestmentStrategy.description
        });
        // TODO: add axios web service call for saving investment strategy changes (create/update)
    }
}