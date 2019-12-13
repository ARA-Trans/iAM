import {AxiosPromise} from 'axios';
import {PerformanceLibrary} from '@/shared/models/iAM/performance';
import {axiosInstance, nodejsAxiosInstance} from '@/shared/utils/axios-instance';
import {convertFromVueToMongo} from '@/shared/utils/mongo-model-conversion-utils';

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
    static saveScenarioPerformanceLibrary(saveScenarioPerformanceLibraryData: PerformanceLibrary): AxiosPromise {
        return axiosInstance.post('/api/SaveScenarioPerformanceLibrary', saveScenarioPerformanceLibraryData);
    }
}
