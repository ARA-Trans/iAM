import {AxiosPromise} from 'axios';
import {axiosInstance} from '@/shared/utils/axios-instance';
// TODO: remove MockAdapter code when api is implemented
import {mockAdapter, mockAxiosInstance} from '@/shared/utils/axios-mock-adapter-instance';
import {Target} from '@/shared/models/iAM/target';

export default class TargetService {
    /**
     * Gets target data
     * @param selectedScenarioId Scenario object id
     */
    static getTargets(selectedScenarioId: number): AxiosPromise {
        // TODO: remove MockAdapter code when api is implemented
        mockAdapter
            .onGet(`/api/GetTargets?selectedScenarioId=${selectedScenarioId}`)
            .reply((config: any) => {
                return [200, []];
            });
        // TODO: replace mockAxiosInstance with axiosInstance
        return mockAxiosInstance.get(`/api/GetTargets?selectedScenarioId=${selectedScenarioId}`);
    }

    /**
     * Saves target data
     * @param targets List of Target objects
     */
    static saveTargets(targets: Target[]): AxiosPromise {
        // TODO: remove MockAdapter code when api is implemented
        mockAdapter
            .onPost('/api/SaveTargets')
            .reply((config: any) => {
                return [200, targets];
            });
        // TODO: replace mockAxiosInstance with axiosInstance
        return mockAxiosInstance.post('/api/SaveTargets', targets);
    }
}