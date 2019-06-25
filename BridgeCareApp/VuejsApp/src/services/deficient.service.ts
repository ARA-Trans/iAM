import {AxiosPromise} from 'axios';
import {axiosInstance} from '@/shared/utils/axios-instance';
import {Deficient} from '@/shared/models/iAM/deficient';

export default class DeficientService {
    /**
     * Gets deficient data
     * @param selectedScenarioId Scenario object id
     */
    static getDeficients(selectedScenarioId: number): AxiosPromise {
        return axiosInstance.get(`/api/GetDeficients?selectedScenarioId=${selectedScenarioId}`);
    }

    /**
     * Saves deficient data
     * @param selectedScenarioId Scenario id
     * @param deficients List of Deficient objects
     */
    static saveDeficients(selectedScenarioId: number, deficients: Deficient[]): AxiosPromise {
        return axiosInstance.post(`/api/SaveDeficients?selectedScenarioId=${selectedScenarioId}`, deficients);
    }
}