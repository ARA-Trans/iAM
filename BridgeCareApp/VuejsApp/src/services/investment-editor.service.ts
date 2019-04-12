import axios from 'axios';
import {InvestmentStrategy} from '@/shared/models/iAM/investment';
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
     * Creates an investment strategy
     * @param createdInvestmentStrategy The investment strategy create data
     */
    createInvestmentStrategy(createdInvestmentStrategy: InvestmentStrategy): Promise<InvestmentStrategy> {
        return Promise.resolve<InvestmentStrategy>(createdInvestmentStrategy);
        // TODO: add axios web service call for creating an investment strategy
    }

    /**
     * Updates an investment strategy
     * @param updatedInvestmentStrategy The investment strategy update data
     */
    updateInvestmentStrategy(updatedInvestmentStrategy: InvestmentStrategy): Promise<InvestmentStrategy> {
        return Promise.resolve<InvestmentStrategy>(updatedInvestmentStrategy);
        // TODO: add axios web service call for updating an investment strategy
    }
}