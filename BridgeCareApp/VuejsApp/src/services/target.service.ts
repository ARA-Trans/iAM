import {AxiosPromise} from 'axios';
import {axiosInstance, nodejsAxiosInstance} from '@/shared/utils/axios-instance';
import {TargetLibrary} from '@/shared/models/iAM/target';
import {convertFromVueToMongo} from '@/shared/utils/mongo-model-conversion-utils';

export default class TargetService {
    /**
     * Gets target libraries data
     */
    static getTargetLibraries(): AxiosPromise {
        return nodejsAxiosInstance.get('/api/GetTargetLibraries');
    }

    /**
     * Created a target library
     * @param createdTargetLibrary The target library create data
     */
    static createTargetLibrary(createdTargetLibrary: TargetLibrary): AxiosPromise {
        return nodejsAxiosInstance.post('/api/CreateTargetLibrary', convertFromVueToMongo(createdTargetLibrary));
    }

    /**
     * Updates a target library
     * @param updatedTargetLibrary The target library update data
     */
    static updateTargetLibrary(updatedTargetLibrary: TargetLibrary): AxiosPromise {
        return nodejsAxiosInstance.put('/api/UpdateTargetLibrary', convertFromVueToMongo(updatedTargetLibrary));
    }

    /**
     * Deletes a target library
     * @param targetLibrary The target library to delete
     */
    static deleteTargetLibrary(targetLibrary: TargetLibrary): AxiosPromise {
        return nodejsAxiosInstance.delete(`/api/DeleteTargetLibrary/${targetLibrary.id}`);
    }

    /**
     * Gets a scenario's target library data
     * @param selectedScenarioId Scenario object id
     */
    static getScenarioTargetLibrary(selectedScenarioId: number): AxiosPromise {
        return axiosInstance.get(`/api/GetScenarioTargetLibrary/${selectedScenarioId}`);
    }

    /**
     * Upserts a scenario's target library data
     * @param scenarioTargetLibraryData Scenario target library upsert data
     */
    static saveScenarioTargetLibrary(scenarioTargetLibraryData: TargetLibrary, objectIdMOngoDBForScenario: string): AxiosPromise {
        // Node API call is to update last modified date. (THe date is set in the nodejs app)
        nodejsAxiosInstance.put(`/api/UpdateMongoScenario/${objectIdMOngoDBForScenario}`);
        return axiosInstance.post('/api/SaveScenarioTargetLibrary', scenarioTargetLibraryData);
    }
}