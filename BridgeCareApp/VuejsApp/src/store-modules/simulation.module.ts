import {Simulation} from '../models/simulation';
import SimulationService from '../services/simulation.service';

const state = {
    simulations: [] as Simulation[]
};

const mutations = {
    // @ts-ignore
    simulationsMutator(state, simulations) {
        state.simulations = simulations;
    }
};

const actions = {
    // @ts-ignore
    async getSimulations({ commit }, payload) {
        await new SimulationService().getSimulations(payload.networkId)
            .then((simulations: Simulation[]) => commit('simulationsMutator', simulations))
            .catch((error: any) => console.log(error));
    },
    // @ts-ignore
    runSimulation({commit}, payload) {
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
