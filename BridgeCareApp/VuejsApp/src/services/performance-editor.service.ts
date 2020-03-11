import {AxiosPromise, AxiosResponse} from 'axios';
import {PerformanceLibrary} from '@/shared/models/iAM/performance';
import {axiosInstance, nodejsAxiosInstance} from '@/shared/utils/axios-instance';
import {convertFromVueToMongo} from '@/shared/utils/mongo-model-conversion-utils';
import { hasValue } from '@/shared/utils/has-value-util';


export default class PerformanceEditorService {
    /**
     * Gets all performance Libraries a user can read/edit
     */
    static getPerformanceLibraries(): AxiosPromise {
        return nodejsAxiosInstance.get('/api/GetPerformanceLibraries');
    }

    /**
     * Creates a performance library
     * @param createPerformanceLibraryData The performance library create data
     */
    static createPerformanceLibrary(createPerformanceLibraryData: PerformanceLibrary): AxiosPromise {
        return nodejsAxiosInstance.post('/api/CreatePerformanceLibrary', convertFromVueToMongo(createPerformanceLibraryData));
    }

    /**
     * Updates a performance library
     * @param updatePerformanceLibraryData The performance library update data
     */
    static updatePerformanceLibrary(updatePerformanceLibraryData: PerformanceLibrary): AxiosPromise {
        return nodejsAxiosInstance.put('/api/UpdatePerformanceLibrary', convertFromVueToMongo(updatePerformanceLibraryData));
    }

    /**
     * Deletes a performance library
     * @param performanceLibraryData The performance library to delete
     */
    static deletePerformanceLibrary(performanceLibraryData: PerformanceLibrary): AxiosPromise {
        return nodejsAxiosInstance.delete(`/api/DeletePerformanceLibrary/${performanceLibraryData.id}`);
    }

    /**
     * Gets a scenario's performance library
     * @param selectedScenarioId Scenario object id
     */
    static getScenarioPerformanceLibrary(selectedScenarioId: number): AxiosPromise {
        return axiosInstance.get(`/api/GetScenarioPerformanceLibrary/${selectedScenarioId}`);
    }

    /**
     * Saves a scenario performance library
     * @param saveScenarioPerformanceLibraryData The scenario performance library upsert data
     */
    static saveScenarioPerformanceLibrary(saveScenarioPerformanceLibraryData: PerformanceLibrary, objectIdMOngoDBForScenario: string): AxiosPromise {
        // Node API call is to update last modified date. (THe date is set in the nodejs app)
        nodejsAxiosInstance.put(`/api/UpdateMongoScenario/${objectIdMOngoDBForScenario}`);
        return axiosInstance.post('/api/SaveScenarioPerformanceLibrary', saveScenarioPerformanceLibraryData);
    }
}
