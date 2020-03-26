import {AxiosPromise} from 'axios';
import {Analysis} from '@/shared/models/iAM/scenario';
import {axiosInstance, nodejsAxiosInstance} from '@/shared/utils/axios-instance';

export default class AnalysisEditorService {
    /**
     * Gets a scenario's analysis data
     * @param selectedScenarioId A scenario's id
     */
    static getScenarioAnalysisData(selectedScenarioId: number): AxiosPromise {
        return axiosInstance.get(`/api/GetScenarioAnalysisData/${selectedScenarioId}`);
    }

    /**
     * Saves a scenario's analysis data
     * @param scenarioAnalysisData A scenario's analysis data
     */
    static saveScenarioAnalysisData(scenarioAnalysisData: Analysis, objectIdMOngoDBForScenario: string): AxiosPromise {
        // Node API call is to update last modified date. (THe date is set in the nodejs app)
        nodejsAxiosInstance.put(`/api/UpdateMongoScenario/${objectIdMOngoDBForScenario}`);
        return axiosInstance.post('/api/SaveScenarioAnalysisData', scenarioAnalysisData);
    }
}
