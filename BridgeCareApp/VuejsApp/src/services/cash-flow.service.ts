import {axiosInstance, nodejsAxiosInstance} from '@/shared/utils/axios-instance';
import {AxiosPromise} from 'axios';
import {CashFlowLibrary} from '@/shared/models/iAM/cash-flow';
import {convertFromVueToMongo} from '@/shared/utils/mongo-model-conversion-utils';

export default class CashFlowService {
    /**
     * Gets all cash flow libraries
     */
    static getCashFlowLibraries(): AxiosPromise {
        return nodejsAxiosInstance.get('/api/GetCashFlowLibraries');
    }

    /**
     * Creates a cash flow library
     * @param createdCashFlowLibrary Cash flow library create data
     */
    static createCashFlowLibrary(createdCashFlowLibrary: CashFlowLibrary): AxiosPromise {
        return nodejsAxiosInstance.post('/api/CreateCashFlowLibrary', convertFromVueToMongo(createdCashFlowLibrary));
    }

    /**
     * Updates a cash flow library
     * @param updatedCashFlowLibrary Cash flow library update data
     */
    static updateCashFlowLibrary(updatedCashFlowLibrary: CashFlowLibrary): AxiosPromise {
        return nodejsAxiosInstance.put('/api/UpdateCashFlowLibrary', convertFromVueToMongo(updatedCashFlowLibrary));
    }

    /**
     * Deletes a cash flow library
     * @param cashFlowLibrary The cash flow library to delete
     */
    static deleteCashFlowLibrary(cashFlowLibrary: CashFlowLibrary): AxiosPromise {
        return nodejsAxiosInstance.delete(`/api/DeleteCashFLowLibrary/${cashFlowLibrary.id}`);
    }

    /**
     * Gets a scenario's cash flow library data
     * @param selectedScenarioId Selected scenario's identifier
     */
    static getScenarioCashFlowLibrary(selectedScenarioId: number): AxiosPromise {
        return axiosInstance.get(`/api/GetScenarioCashFlowLibrary/${selectedScenarioId}`);
    }

    /**
     * Upserts a scenario's cash flow library data
     * @param saveScenarioCashFlowLibraryData Scenario's cash flow library upsert data
     */
    static saveScenarioCashFlowLibrary(saveScenarioCashFlowLibraryData: CashFlowLibrary, objectIdMOngoDBForScenario: string): AxiosPromise {
        // Node API call is to update last modified date. (THe date is set in the nodejs app)
        nodejsAxiosInstance.put(`/api/UpdateMongoScenario/${objectIdMOngoDBForScenario}`);
        return axiosInstance.post('/api/SaveScenarioCashFlowLibrary', saveScenarioCashFlowLibraryData);
    }
}