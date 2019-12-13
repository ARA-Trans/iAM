import {axiosInstance, nodejsAxiosInstance} from '@/shared/utils/axios-instance';
import {AxiosPromise} from 'axios';
import {InvestmentLibrary} from '@/shared/models/iAM/investment';
import {convertFromVueToMongo} from '@/shared/utils/mongo-model-conversion-utils';
import {getAuthorizationHeader} from '@/shared/utils/authorization-header';

export default class InvestmentEditorService {
    /**
     * Gets all investment libraries
     */
    static getInvestmentLibraries(): AxiosPromise {
        return nodejsAxiosInstance.get('/api/GetInvestmentLibraries', {headers: getAuthorizationHeader()});
    }

    /**
     * Creates an investment library
     * @param createdInvestmentLibrary The investment library create data
     */
    static createInvestmentLibrary(createdInvestmentLibrary: InvestmentLibrary): AxiosPromise {
        return nodejsAxiosInstance.post('/api/CreateInvestmentLibrary', convertFromVueToMongo(createdInvestmentLibrary), {headers: getAuthorizationHeader()});
    }

    /**
     * Updates an investment library
     * @param updatedInvestmentLibrary The investment library update data
     */
    static updateInvestmentLibrary(updatedInvestmentLibrary: InvestmentLibrary): AxiosPromise {
        return nodejsAxiosInstance.put('/api/UpdateInvestmentLibrary', convertFromVueToMongo(updatedInvestmentLibrary), {headers: getAuthorizationHeader()});
    }

    /**
     * Gets a scenario's investment library data
     * @param selectedScenarioId Scenario id to use in finding a scenario's investment library data
     */
    static getScenarioInvestmentLibrary(selectedScenarioId: number): AxiosPromise {
        return axiosInstance.get(`/api/GetScenarioInvestmentLibrary/${selectedScenarioId}`, {headers: getAuthorizationHeader()});
    }

    /**
     * Upserts a scenario's investment library data
     * @param saveScenarioInvestmentLibraryData The scenario investment library upsert data
     */
    static saveScenarioInvestmentLibrary(saveScenarioInvestmentLibraryData: InvestmentLibrary): AxiosPromise {
        return axiosInstance.post('/api/SaveScenarioInvestmentLibrary', saveScenarioInvestmentLibraryData, {headers: getAuthorizationHeader()});
    }
}