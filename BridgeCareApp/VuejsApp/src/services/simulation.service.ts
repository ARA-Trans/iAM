import axios, {AxiosPromise} from 'axios';
import {Simulation} from '@/shared/models/iAM/simulation';

axios.defaults.baseURL = process.env.VUE_APP_URL;

export default class SimulationService {
    /**
     * Gets simulations for a specified network
     * @param networkId
     */
    getSimulations(networkId: number): AxiosPromise<Simulation[]> {
        return axios.get<Simulation[]>(`/api/Simulations/${networkId}`);
    }

    /**
     * Runs a specified simulation
     * @param networkId
     * @param networkName
     * @param simulationId
     * @param simulationName
     */
    runSimulation(networkId: number, networkName: string, simulationId: number, simulationName: string): AxiosPromise<any> {
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
