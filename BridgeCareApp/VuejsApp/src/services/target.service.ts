import {AxiosPromise} from 'axios';
import {axiosInstance} from '@/shared/utils/axios-instance';
import {TargetLibrary} from '@/shared/models/iAM/target';

export default class TargetService {
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
    static saveScenarioTargetLibrary(scenarioTargetLibraryData: TargetLibrary): AxiosPromise {
        return axiosInstance.post('/api/SaveScenarioTargetLibrary', scenarioTargetLibraryData);
    }
}