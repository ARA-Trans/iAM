import Vue from 'vue';
import axios, {AxiosResponse} from 'axios';
import {Simulation} from '@/shared/models/iAM/simulation';

axios.defaults.baseURL = process.env.VUE_APP_URL;

export default class SimulationService extends Vue {
    getSimulations(networkId: number): Promise<Simulation[]> {
        return axios.get<Simulation>(`/api/Simulations/${networkId}`)
            .then((response: AxiosResponse) => response.data);
    }

    runSimulation(networkId: number, networkName: string, simulationId: number, simulationName: string): Promise<any> {
        return axios.post<any>('/api/RunSimulation',
            {
                NetworkId: networkId,
                SimulationId: simulationId,
                NetworkName: networkName,
                SimulationName: simulationName
            }
        ).then((response: AxiosResponse) => response.data);
    }
}
