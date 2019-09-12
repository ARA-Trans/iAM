import {AxiosPromise} from 'axios';
import {axiosInstance} from '@/shared/utils/axios-instance';
import {DeficientLibrary} from '@/shared/models/iAM/deficient';

export default class DeficientService {
    /**
     * Gets scenario deficient library data
     * @param selectedScenarioId Scenario object id
     */
    static getScenarioDeficientLibrary(selectedScenarioId: number): AxiosPromise {
        return axiosInstance.get(`/api/GetScenarioDeficientLibrary/${selectedScenarioId}`);
    }

    /**
     * Saves scenario deficient library data
     * @param scenarioDeficientLibraryData Scenario deficient library data
     */
    static saveScenarioDeficientLibrary(scenarioDeficientLibraryData: DeficientLibrary): AxiosPromise {
        return axiosInstance.post('/api/SaveScenarioDeficientLibrary', scenarioDeficientLibraryData);
    }
}