import {AxiosPromise} from 'axios';
import {Analysis} from '@/shared/models/iAM/scenario';
import {axiosInstance} from '@/shared/utils/axios-instance';

export default class AnalysisEditorService {
    /**
     * Gets a scenario's analysis data
     * @param selectedScenarioId A scenario's id
     */
    static getScenarioAnalysisData(selectedScenarioId: number): AxiosPromise<Analysis> {
        return axiosInstance.get<Analysis>(`/api/GetScenarioAnalysisData/${selectedScenarioId}`);
    }

    /**
     * Saves a scenario's analysis data
     * @param scenarioAnalysisData A scenario's analysis data
     */
    static saveScenarioAnalysisData(scenarioAnalysisData: Analysis): AxiosPromise<any> {
        return axiosInstance.post<any>('/api/SaveScenarioAnalysisData', scenarioAnalysisData);
    }
}
