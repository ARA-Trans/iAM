import {Simulation} from '@/shared/models/iAM/simulation';
import SimulationService from '../services/simulation.service';

const state = {
    simulations: [] as Simulation[]
};

const mutations = {
    simulationsMutator(state: any, simulations: Simulation[]) {
        state.simulations = simulations;
    }
};

const actions = {
    async getSimulations({commit}: any, payload: any) {
        await new SimulationService().getSimulations(payload.networkId)
            .then((simulations: Simulation[]) => commit('simulationsMutator', simulations))
            .catch((error: any) => console.log(error));
    },
    runSimulation({commit}: any, payload: any) {
        new SimulationService().runSimulation(payload.networkId, payload.networkName, payload.simulationId, payload.simulationName)
            .then((simulation: any) => console.log(simulation))
            .catch((error: any) => console.log(error));
    }
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations
};
