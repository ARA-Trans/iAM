import {axiosInstance, nodejsAxiosInstance} from '@/shared/utils/axios-instance';
import {AxiosPromise, AxiosResponse} from 'axios';
import {InvestmentLibrary} from '@/shared/models/iAM/investment';
import {convertFromVueToMongo} from '@/shared/utils/mongo-model-conversion-utils';
import { Scenario } from '@/shared/models/iAM/scenario';
import { hasValue } from '@/shared/utils/has-value-util';

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
        return nodejsAxiosInstance.post('/api/CreateInvestmentLibrary', convertFromVueToMongo(createdInvestmentLibrary));
    }

    /**
     * Updates an investment library
     * @param updatedInvestmentLibrary The investment library update data
     */
    static updateInvestmentLibrary(updatedInvestmentLibrary: InvestmentLibrary): AxiosPromise {
        return nodejsAxiosInstance.put('/api/UpdateInvestmentLibrary', convertFromVueToMongo(updatedInvestmentLibrary));
    }

    /**
     * Deletes an investment library
     * @param investmentLibrary The investment library to delete
     */
    static deleteInvestmentLibrary(investmentLibrary: InvestmentLibrary): AxiosPromise {
        return nodejsAxiosInstance.delete(`/api/DeleteInvestmentLibrary/${investmentLibrary.id}`);
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
    static saveScenarioInvestmentLibrary(saveScenarioInvestmentLibraryData: InvestmentLibrary, objectIdMOngoDBForScenario: string): AxiosPromise {
        // Node API call is to update last modified date. (THe date is set in the nodejs app)
        nodejsAxiosInstance.put(`/api/UpdateMongoScenario/${objectIdMOngoDBForScenario}`);
        return axiosInstance.post('/api/SaveScenarioInvestmentLibrary', saveScenarioInvestmentLibraryData);
    }
}