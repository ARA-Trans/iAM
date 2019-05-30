import {AxiosPromise} from 'axios';
import {Analysis, emptyAnalysis} from '@/shared/models/iAM/scenario';
import {axiosInstance} from '@/shared/utils/axios-instance';
// TODO: remove MockAdapter code when api is implemented
import {mockAdapter, mockAxiosInstance} from '@/shared/utils/axios-mock-adapter-instance';

export default class AnalysisEditorService {
    /**
     * Gets a scenario's analysis data
     * @param selectedScenarioId A scenario's id
     */
    static getScenarioAnalysisData(selectedScenarioId: number): AxiosPromise<Analysis> {
        // TODO: remove MockAdapter code when api is implemented
        mockAdapter
            .onGet('/api/GetScenarioAnalysisData')
            .reply((config: any) => {
                return [200, emptyAnalysis];
            });
        // TODO: replace mockAxiosInstance with axiosInstance
        return mockAxiosInstance.get<Analysis>('/api/GetScenarioAnalysisData', {
            params: {'selectedScenarioId': selectedScenarioId}
        });
    }

    /**
     * Saves a scenario's analysis data
     * @param scenarioAnalysisData A scenario's analysis data
     */
    static saveScenarioAnalysisData(scenarioAnalysisData: Analysis): AxiosPromise<any> {
        // TODO: remove MockAdapter code when api is implemented
        mockAdapter
            .onGet('/api/SaveScenarioAnalysisData')
            .reply((config: any) => {
                return [201];
            });
        // TODO: replace mockAxiosInstance with axiosInstance
        return mockAxiosInstance.post<any>('/api/SaveScenarioAnalysisData', scenarioAnalysisData);
    }
}
