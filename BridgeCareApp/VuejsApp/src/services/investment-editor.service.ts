import axios from 'axios';
import {InvestmentStrategy} from '@/shared/models/iAM/investment';
import { Simulation } from '@/shared/models/iAM/simulation';

axios.defaults.baseURL = process.env.VUE_APP_URL;

export default class InvestmentEditorService {

    getInvestmentForScenario(selectedScenario: number): Promise<InvestmentStrategy> {
        return axios.get(`/api/GetInvestmentStrategies/${selectedScenario}`)
            .then((response: any) => {
                return response.data as Promise<InvestmentStrategy>;
            });
    }

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