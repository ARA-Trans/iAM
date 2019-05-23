import {AxiosPromise} from 'axios';
import {axiosInstance} from '@/shared/utils/axios-instance';

export default class ScenarioService {
    /**
     * Gets all scenarios
     */
    static getScenarios(): AxiosPromise<any[]> {
        return axiosInstance.get<any[]>('/api/Simulations');
    }

    /**
     * Creates a new scenario
     * @param networkId
     * @param simulationName
     */
    static createScenario(networkId: number, simulationName: string): AxiosPromise<any> {
        return axiosInstance.post<any>(`/api/CreateNewSimulation/${networkId}/${simulationName}`);
    }

    /**
     * Runs a specified simulation
     * @param networkId
     * @param networkName
     * @param simulationId
     * @param simulationName
     */
    static runScenarioSimulation(networkId: number, networkName: string, simulationId: number, simulationName: string): AxiosPromise<any> {
        return axiosInstance.post<any>('/api/RunSimulation',
            {
                NetworkId: networkId,
                SimulationId: simulationId,
                NetworkName: networkName,
                SimulationName: simulationName
            }
        );
    }
}