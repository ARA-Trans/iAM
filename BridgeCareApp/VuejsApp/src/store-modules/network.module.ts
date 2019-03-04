import {Network} from '../models/network';
import NetworkService from '../services/network.service';

const state = {
    networks: [] as Network[]
};

const mutations = {
    // @ts-ignore
    networksMutator(state, networks) {
        state.networks = networks;
    }
};

const actions = {
    // @ts-ignore
    async getNetworks({ commit }) {
        await new NetworkService().getNetworks()
            .then((networks: Network[]) => commit('networksMutator', networks))
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
