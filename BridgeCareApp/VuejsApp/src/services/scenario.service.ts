import axios, {AxiosResponse} from 'axios';
import {Analysis, emptyAnalysis, Scenario} from '@/shared/models/iAM/scenario';

axios.defaults.baseURL = process.env.VUE_APP_URL;

export default class ScenarioService {
    getScenarioAnalysisData(scenarioId: number): Promise<Analysis> {
        return Promise.resolve<Analysis>(emptyAnalysis);
        // TODO: add axios web service call to get a scenario's analysis data
    }

    applyAnalysisDataToScenario(analysis: Analysis): Promise<boolean> {
        return Promise.resolve<boolean>(true);
        // TODO: add axios web service call to upsert analysis data for a scenario
    }

    getScenarios(): Promise<Scenario[]> {
        return axios.get<Scenario[]>('/api/Simulations')
            .then((response: AxiosResponse) => response.data);
    }

    createNewScenario(networkId: number, simulationName: string): Promise<any> {
        return axios.post<any>(`/api/CreateNewSimulation/${networkId}/${simulationName}`)
            .then((response: AxiosResponse) => response.data);
    }

    uploadCommittedProjectsFiles(files: File[]): Promise<any> {
        return Promise.resolve<any>({} as any);
        // TODO: add axios web service call to upload excel file
    }
}