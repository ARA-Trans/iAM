import axios from 'axios';
import {InvestmentStrategy} from '@/shared/models/iAM/investment';
import {mockInvestmentStrategies} from '@/shared/utils/mock-data';
import { Simulation } from '@/shared/models/iAM/simulation';
import concat from 'ramda/es/concat';

axios.defaults.baseURL = process.env.VUE_APP_URL;

export default class InvestmentEditorService {
    /**
     * Gets all investment strategies a user can read/edit
     */
    //getInvestmentStrategies(): Promise<InvestmentStrategy[]> {
    //    return Promise.resolve<InvestmentStrategy[]>(mockInvestmentStrategies);
    //    // TODO: add axios web service call for investment strategies
    //}

    getInvestmentForScenario(selectedScenario: Simulation): Promise<InvestmentStrategy> {
        //return Promise.resolve<InvestmentStrategy>(mockInvestmentStrategies);
        let scenario = {
            NetworkId: selectedScenario.networkId,
            SimulationId: selectedScenario.simulationId,
            NetworkName: selectedScenario.networkName,
            SimulationName: selectedScenario.simulationName
        };
        return axios.get(`/api/GetInvestmentStrategies/${selectedScenario.simulationId}`)
            .then((response: any) => {
                return response.data as Promise<InvestmentStrategy>;
            });
    }

    /**
     * Creates an investment strategy
     * @param createdInvestmentStrategy The investment strategy create data
     */
    //createInvestmentStrategy(createdInvestmentStrategy: InvestmentStrategy): Promise<InvestmentStrategy> {
    //    return Promise.resolve<InvestmentStrategy>(createdInvestmentStrategy);
    //    // TODO: add axios web service call for creating an investment strategy
    //}

    /**
     * Updates an investment strategy
     * @param updatedInvestmentStrategy The investment strategy update data
     */
    updateInvestmentScenario(updatedInvestmentScenario: InvestmentStrategy): Promise<InvestmentStrategy> {
        return axios({
            method: 'post',
            url: '/api/SaveInvestmentStrategy',
            data: {
                SimulationId: updatedInvestmentScenario.id,
                Name: updatedInvestmentScenario.name,
                InflationRate: updatedInvestmentScenario.inflationRate,
                DiscountRate: updatedInvestmentScenario.discountRate,
                Description: updatedInvestmentScenario.description,
                BudgetNamesByOrder: updatedInvestmentScenario.budgetOrder,
                YearlyBudgets: updatedInvestmentScenario.budgetYears
            }
        }).then((response: any) => {
            return response;
            })
            .catch((error: any) => { return error.response; });
    }
}