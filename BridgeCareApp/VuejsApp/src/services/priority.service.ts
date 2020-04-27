import {AxiosPromise} from 'axios';
import {axiosInstance, nodejsAxiosInstance} from '@/shared/utils/axios-instance';
import {PriorityLibrary} from '@/shared/models/iAM/priority';
import {convertFromVueToMongo} from '@/shared/utils/mongo-model-conversion-utils';

export default class PriorityService {
    /**
     * Gets priority libraries data
     */
    static getPriorityLibraries(): AxiosPromise {
        return nodejsAxiosInstance.get('/api/GetPriorityLibraries');
    }

    /**
     * Creates a priority library
     * @param createdPriorityLibrary The priority library create data
     */
    static createPriorityLibrary(createdPriorityLibrary: PriorityLibrary): AxiosPromise {
        return nodejsAxiosInstance.post('/api/CreatePriorityLibrary', convertFromVueToMongo(createdPriorityLibrary));
    }

    /**
     * Updates a priority library
     * @param updatedPriorityLibrary The priority library update data
     */
    static updatePriorityLibrary(updatedPriorityLibrary: PriorityLibrary): AxiosPromise {
        return nodejsAxiosInstance.put('/api/UpdatePriorityLibrary', convertFromVueToMongo(updatedPriorityLibrary));
    }

    /**
     * Deletes a priority library
     * @param priorityLibraryData The priority library to delete
     */
    static deletePriorityLibrary(priorityLibraryData: PriorityLibrary): AxiosPromise {
        return nodejsAxiosInstance.delete(`/api/DeletePriorityLibrary/${priorityLibraryData.id}`);
    }

    /**
     * Gets a scenario's priority library data
     * @param selectedScenarioId Scenario id to use in finding a scenario's priority library data
     */
    static getScenarioPriorityLibrary(selectedScenarioId: number): AxiosPromise {
        return axiosInstance.get(`/api/GetScenarioPriorityLibrary/${selectedScenarioId}`);
    }

    /**
     * Upserts a scenario's priority library data
     * @param saveScenarioPriorityLibraryData The scenario priority library upsert data
     */
    static saveScenarioPriorityLibrary(saveScenarioPriorityLibraryData: PriorityLibrary, objectIdMOngoDBForScenario: string): AxiosPromise {
        // Node API call is to update last modified date. (THe date is set in the nodejs app)
        nodejsAxiosInstance.put(`/api/UpdateMongoScenario/${objectIdMOngoDBForScenario}`);
        return axiosInstance.post('/api/SaveScenarioPriorityLibrary', saveScenarioPriorityLibraryData);
    }
}