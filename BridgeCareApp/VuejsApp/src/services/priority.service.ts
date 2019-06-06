import {AxiosPromise} from 'axios';
import {axiosInstance} from '@/shared/utils/axios-instance';
// TODO: remove MockAdapter code when api is implemented
import {mockAdapter, mockAxiosInstance} from '@/shared/utils/axios-mock-adapter-instance';
import {Priority} from '@/shared/models/iAM/priority';

export default class PriorityService {
    /**
     * Gets priority data
     * @param selectedScenarioId Scenario object id
     */
    static getPriorities(selectedScenarioId: number): AxiosPromise {
        // TODO: remove MockAdapter code when api is implemented
        mockAdapter
            .onGet(`/api/GetPriorities?selectedScenarioId=${selectedScenarioId}`)
            .reply((config: any) => {
                return [200, []];
            });
        // TODO: replace mockAxiosInstance with axiosInstance
        return mockAxiosInstance.get(`/api/GetPriorities?selectedScenarioId=${selectedScenarioId}`);
    }

    /**
     * Saves priority data
     * @param priorities List of Priority objects
     */
    static savePriorities(priorities: Priority[]): AxiosPromise {
        // TODO: remove MockAdapter code when api is implemented
        mockAdapter
            .onPost('/api/SavePriorities')
            .reply((config: any) => {
                return [200, priorities];
            });
        // TODO: replace mockAxiosInstance with axiosInstance
        return mockAxiosInstance.post('/api/SavePriorities', priorities);
    }
}