import Vue from 'vue';
import axios from 'axios';
import {Simulation} from '@/shared/models/iAM/simulation';

axios.defaults.baseURL = process.env.VUE_APP_URL;

export default class SimulationService extends Vue {
    getSimulations(networkId: number): Promise<Simulation[]> {
        return axios.get(`/api/Simulations/${networkId}`)
            .then((response: any) => {
                return response.data as Promise<Simulation[]>;
            });
    }

    runSimulation(networkId: number, networkName: string, simulationId: number, simulationName: string): Promise<any> {
        return axios({
            method: 'post',
            url: '/api/RunSimulation',
            data: {
                NetworkId: networkId,
                SimulationId: simulationId,
                NetworkName: networkName,
                SimulationName: simulationName
            }
        }).then((response: any) => {
            return response;
        });
    }
}
