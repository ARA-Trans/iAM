import axios from 'axios';
import {InvestmentStrategy} from '@/models/iAM/investment';
import {mockInvestmentStrategies} from '@/shared/utils/mock-data';
import {Network} from '@/models/iAM/network';

axios.defaults.baseURL = process.env.VUE_APP_URL;

export default class InvestmentEditorService {
    getInvestmentStrategies(network: Network): Promise<InvestmentStrategy[]> {
        return Promise.resolve<InvestmentStrategy[]>(mockInvestmentStrategies);
        // TODO: add axios web service call for investment strategies
    }
}