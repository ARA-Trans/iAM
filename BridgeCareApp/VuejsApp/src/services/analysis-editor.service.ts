import {AxiosPromise} from 'axios';
import {Analysis, emptyAnalysis} from '@/shared/models/iAM/scenario';
import {axiosInstance} from '@/shared/utils/axios-instance';
// TODO: remove MockAdapter code when api is implemented
import MockAdapter from 'axios-mock-adapter';

export default class AnalysisEditorService {
    /**
     * Gets a scenario's analysis data
     * @param selectedScenarioId A scenario's id
     */
    static getScenarioAnalysisData(selectedScenarioId: number): AxiosPromise<Analysis> {
        // TODO: remove MockAdapter code when api is implemented
        const mockAdapterInstance = new MockAdapter(axiosInstance)
            .onGet(`${axiosInstance.defaults.baseURL}/api/GetScenarioAnalysisData`)
            .reply((config: any) => {
                mockAdapterInstance.restore();
                return [200, emptyAnalysis];
            });
        return axiosInstance.get<Analysis>('/api/GetScenarioAnalysisData', {
            params: {'selectedScenarioId': selectedScenarioId}
        });
    }

    /**
     * Saves a scenario's analysis data
     * @param scenarioAnalysisData A scenario's analysis data
     */
    static saveScenarioAnalysisData(scenarioAnalysisData: Analysis): AxiosPromise<any> {
        // TODO: remove MockAdapter code when api is implemented
        const mockAdapterInstance = new MockAdapter(axiosInstance)
            .onPost(`${axiosInstance.defaults.baseURL}/api/SaveScenarioAnalysisData`)
            .reply((config: any) => {
                mockAdapterInstance.restore();
                return [201];
            });
        return axiosInstance.post<any>('/api/SaveScenarioAnalysisData', scenarioAnalysisData);
    }
}
