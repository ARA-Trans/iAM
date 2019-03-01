import axios from 'axios';
import {InvestmentStrategy, InvestmentStrategyDetail} from '@/models/investment';

axios.defaults.baseURL = process.env.VUE_APP_URL;

export default class InvestmentEditorService {
    getInvestmentStrategies(): Promise<InvestmentStrategy[]> {
        return Promise.resolve<InvestmentStrategy[]>([]);
        // TODO: add axios web service call for investment strategies
    }

    getInvestmentStrategyDetails(id: number): Promise<InvestmentStrategyDetail[]> {
        return Promise.resolve<InvestmentStrategyDetail[]>([]);
        // TODO: add axios web service call for investment strategy detail
    }
}