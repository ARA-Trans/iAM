import {AxiosPromise} from 'axios';
import {Analysis} from '@/shared/models/iAM/scenario';
import {axiosInstance} from '@/shared/utils/axios-instance';
import {getAuthHeader} from '@/shared/utils/authentication-header';

export default class AnalysisEditorService {
    /**
     * Gets a scenario's analysis data
     * @param selectedScenarioId A scenario's id
     */
    static getScenarioAnalysisData(selectedScenarioId: number): AxiosPromise {
        return axiosInstance.get(`/api/GetScenarioAnalysisData/${selectedScenarioId}`, {headers: getAuthHeader()});
    }

    /**
     * Saves a scenario's analysis data
     * @param scenarioAnalysisData A scenario's analysis data
     */
    static saveScenarioAnalysisData(scenarioAnalysisData: Analysis): AxiosPromise {
        return axiosInstance.post('/api/SaveScenarioAnalysisData', scenarioAnalysisData, {headers: getAuthHeader()});
    }
}
