import {Network} from '@/shared/models/iAM/network';
import NetworkService from '../services/network.service';
import {clone} from 'ramda';
import {AxiosResponse} from 'axios';

const state = {
    networks: [] as Network[]
};

const mutations = {
    networksMutator(state: any, networks: Network[]) {
        state.networks = clone(networks);
    }
};

const actions = {
    async getNetworks({commit}: any) {
        await NetworkService.getNetworks()
            .then((response: AxiosResponse<Network[]>) => {
                commit('networksMutator', response.data);
            });
    }
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations
};
