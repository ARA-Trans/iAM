import {Network} from '@/shared/models/iAM/network';
import NetworkService from '../services/network.service';
import {clone} from 'ramda';
import {AxiosResponse} from 'axios';
import {hasValue} from '@/shared/utils/has-value-util';

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
                if (hasValue(response, 'data')) {
                    commit('networksMutator', response.data);
                }
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
