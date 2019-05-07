import axios from 'axios';
import {InvestmentLibrary} from '@/shared/models/iAM/investment';

axios.defaults.baseURL = process.env.VUE_APP_URL;

export default class InvestmentEditorService {
    /**
     * Gets a scenario's investment library data
     * @param selectedScenarioId
     */
    getScenarioInvestmentLibrary(selectedScenarioId: number): Promise<InvestmentLibrary> {
        return axios.get(`/api/GetInvestmentStrategies/${selectedScenarioId}`)
            .then((response: any) => {
                return response.data as Promise<InvestmentLibrary>;
            });
    }

    /**
     * Upserts a scenario's investment library data
     * @param updatedScenarioInvestmentLibrary The scenario investment library update data
     */
    updateScenarioInvestmentLibrary(updatedScenarioInvestmentLibrary: InvestmentLibrary): Promise<InvestmentLibrary> {
        return axios({
            method: 'post',
            url: '/api/SaveInvestmentStrategy',
            data: {
                SimulationId: updatedScenarioInvestmentLibrary.id,
                Name: updatedScenarioInvestmentLibrary.name,
                InflationRate: updatedScenarioInvestmentLibrary.inflationRate,
                DiscountRate: updatedScenarioInvestmentLibrary.discountRate,
                Description: updatedScenarioInvestmentLibrary.description,
                BudgetNamesByOrder: updatedScenarioInvestmentLibrary.budgetOrder,
                YearlyBudgets: updatedScenarioInvestmentLibrary.budgetYears
            }
        }).then((response: any) => {
            return response;
        });
    }
}