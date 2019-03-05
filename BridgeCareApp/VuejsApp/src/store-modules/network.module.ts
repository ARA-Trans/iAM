import {Network} from '@/models/iAM/network';
import NetworkService from '../services/network.service';

const state = {
    networks: [] as Network[]
};

const mutations = {
    networksMutator(state: any, networks: Network[]) {
        state.networks = networks;
    }
};

const actions = {
    getNetworks({commit}: any) {
        new NetworkService().getNetworks()
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
