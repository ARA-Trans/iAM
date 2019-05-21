import axios, {AxiosPromise, AxiosResponse} from 'axios';
import {Analysis, emptyAnalysis, Scenario} from '@/shared/models/iAM/scenario';

axios.defaults.baseURL = process.env.VUE_APP_URL;

export default class ScenarioService {
    static getScenarioAnalysisData(scenarioId: number): Promise<Analysis> {
        return Promise.resolve<Analysis>(emptyAnalysis);
        // TODO: add axios web service call to get a scenario's analysis data
    }

    static applyAnalysisDataToScenario(analysis: Analysis): Promise<boolean> {
        return Promise.resolve<boolean>(true);
        // TODO: add axios web service call to upsert analysis data for a scenario
    }

    static getScenarios(): AxiosPromise<any[]> {
        return axios.get<any[]>('/api/Simulations');
    }

    static createNewScenario(networkId: number, simulationName: string): AxiosPromise<any> {
        return axios.post<any>(`/api/CreateNewSimulation/${networkId}/${simulationName}`);
    }

    static uploadCommittedProjectsFiles(files: File[]): Promise<any> {
        return Promise.resolve<any>({} as any);
        // TODO: add axios web service call to upload excel file
    }

    /**
     * Runs a specified simulation
     * @param networkId
     * @param networkName
     * @param simulationId
     * @param simulationName
     */
    static runSimulation(networkId: number, networkName: string, simulationId: number, simulationName: string): AxiosPromise<any> {
        return axios.post<any>('/api/RunSimulation',
            {
                NetworkId: networkId,
                SimulationId: simulationId,
                NetworkName: networkName,
                SimulationName: simulationName
            }
        );
    }
}