import {AxiosPromise} from 'axios';
import {axiosInstance} from '@/shared/utils/axios-instance';
// TODO: remove MockAdapter code when api is implemented
import {mockAdapter, mockAxiosInstance} from '@/shared/utils/axios-mock-adapter-instance';
import {Deficient} from '@/shared/models/iAM/deficient';

export default class DeficientService {
    /**
     * Gets deficient data
     * @param selectedScenarioId Scenario object id
     */
    static getDeficients(selectedScenarioId: number): AxiosPromise {
        // TODO: remove MockAdapter code when api is implemented
        mockAdapter
            .onGet(`/api/GetDeficients?selectedScenarioId=${selectedScenarioId}`)
            .reply((config: any) => {
                return [200, []];
            });
        // TODO: replace mockAxiosInstance with axiosInstance
        return mockAxiosInstance.get(`/api/GetDeficients?selectedScenarioId=${selectedScenarioId}`);
    }

    /**
     * Saves deficient data
     * @param deficients List of Deficient objects
     */
    static saveDeficients(deficients: Deficient[]): AxiosPromise {
        // TODO: remove MockAdapter code when api is implemented
        mockAdapter
            .onPost('/api/SaveDeficients')
            .reply((config: any) => {
                return [200, deficients];
            });
        // TODO: replace mockAxiosInstance with axiosInstance
        return mockAxiosInstance.post('/api/SaveDeficients', deficients);
    }
}