import {axiosInstance, nodejsAxiosInstance} from '@/shared/utils/axios-instance';
import {AxiosPromise} from 'axios';
import {InvestmentLibrary, InvestmentLibraryBudgetYear} from '@/shared/models/iAM/investment';

const modifyDataForMongoDB = (investmentLibrary: InvestmentLibrary): any => {
    const investmentLibraryData: any = {
        ...investmentLibrary,
        _id: investmentLibrary.id,
        budgetYears: investmentLibrary.budgetYears.map((budgetYear: InvestmentLibraryBudgetYear) => {
            const budgetYearData: any = {...budgetYear, _id: budgetYear.id};
            delete budgetYearData.id;
            return budgetYearData;
        }),
        budgetCriteria: investmentLibrary.budgetCriteria.map((criteria: any) => {
            delete criteria._id;
            return criteria;
        })
    };
    delete investmentLibraryData.id;
    return investmentLibraryData;
};

export default class InvestmentEditorService {
    /**
     * Gets all investment libraries
     */
    static getInvestmentLibraries(): AxiosPromise {
        return nodejsAxiosInstance.get('/api/GetInvestmentLibraries');
    }

    /**
     * Creates an investment library
     * @param createdInvestmentLibrary The investment library create data
     */
    static createInvestmentLibrary(createdInvestmentLibrary: InvestmentLibrary): AxiosPromise {
        return nodejsAxiosInstance.post('/api/CreateInvestmentLibrary', modifyDataForMongoDB(createdInvestmentLibrary));
    }

    /**
     * Updates an investment library
     * @param updatedInvestmentLibrary The investment library update data
     */
    static updateInvestmentLibrary(updatedInvestmentLibrary: InvestmentLibrary): AxiosPromise {
        return nodejsAxiosInstance.put('/api/UpdateInvestmentLibrary', modifyDataForMongoDB(updatedInvestmentLibrary));
    }

    /**
     * Gets a scenario's investment library data
     * @param selectedScenarioId Scenario id to use in finding a scenario's investment library data
     */
    static getScenarioInvestmentLibrary(selectedScenarioId: number): AxiosPromise {
        return axiosInstance.get(`/api/GetScenarioInvestmentLibrary/${selectedScenarioId}`);
    }

    /**
     * Upserts a scenario's investment library data
     * @param saveScenarioInvestmentLibraryData The scenario investment library upsert data
     */
    static saveScenarioInvestmentLibrary(saveScenarioInvestmentLibraryData: InvestmentLibrary): AxiosPromise {
        return axiosInstance.post('/api/SaveScenarioInvestmentLibrary', saveScenarioInvestmentLibraryData);
    }
}