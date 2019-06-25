import {AxiosPromise} from 'axios';
import {axiosInstance} from '@/shared/utils/axios-instance';
import {Target} from '@/shared/models/iAM/target';

export default class TargetService {
    /**
     * Gets target data
     * @param selectedScenarioId Scenario object id
     */
    static getTargets(selectedScenarioId: number): AxiosPromise {
        return axiosInstance.get(`/api/GetTargets?selectedScenarioId=${selectedScenarioId}`);
    }

    /**
     * Saves target data
     * @param selectedScenarioId Scenario id
     * @param targets List of Target objects
     */
    static saveTargets(selectedScenarioId: number, targets: Target[]): AxiosPromise {
        return axiosInstance.post(`/api/SaveTargets?selectedScenarioId=${selectedScenarioId}`, targets);
    }
}