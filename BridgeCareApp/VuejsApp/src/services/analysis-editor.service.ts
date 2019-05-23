import {AxiosPromise} from 'axios';
import {Analysis, emptyAnalysis} from '@/shared/models/iAM/scenario';
import {axiosInstance} from '@/shared/utils/axios-instance';
// TODO: remove mockAdapter code when api is implemented
import {mockAdapterInstance} from '@/shared/utils/mock-adapter-instance';

export default class AnalysisEditorService {
    /**
     * Gets a scenario's analysis data
     * @param selectedScenarioId A scenario's id
     */
    static getScenarioAnalysisData(selectedScenarioId: number): AxiosPromise<Analysis> {
        // TODO: remove mockAdapter code when api is implemented
        mockAdapterInstance.onGet(`${axiosInstance.defaults.baseURL}/api/GetScenarioAnalysisData`)
            .reply(200, emptyAnalysis);
        return axiosInstance.get<Analysis>('/api/GetScenarioAnalysisData', {
            params: {'selectedScenarioId': selectedScenarioId}
        });
    }

    /**
     * Saves a scenario's analysis data
     * @param scenarioAnalysisData A scenario's analysis data
     */
    static saveScenarioAnalysisData(scenarioAnalysisData: Analysis): AxiosPromise<any> {
        // TODO: remove mockAdapter code when api is implemented
        mockAdapterInstance.onPost(`${axiosInstance.defaults.baseURL}/api/SaveScenarioAnalysisData`)
            .reply(201);
        return axiosInstance.post<any>('/api/SaveScenarioAnalysisData', scenarioAnalysisData);
    }
}
