import {AxiosPromise} from 'axios';
import {axiosInstance} from '@/shared/utils/axios-instance';
import {Priority} from '@/shared/models/iAM/priority';

export default class PriorityService {
    /**
     * Gets priority data
     * @param selectedScenarioId Scenario object id
     */
    static getPriorities(selectedScenarioId: number): AxiosPromise {
        return axiosInstance.get(`/api/GetPriorities?selectedScenarioId=${selectedScenarioId}`);
    }

    /**
     * Saves priority data
     * @param selectedScenarioId Scenario id
     * @param priorities List of Priority objects
     */
    static savePriorities(selectedScenarioId: number, priorities: Priority[]): AxiosPromise {
        return axiosInstance.post(`/api/SavePriorities?selectedScenarioId=${selectedScenarioId}`, priorities);
    }
}