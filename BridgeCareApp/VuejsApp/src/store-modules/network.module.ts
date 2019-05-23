import {Network} from '@/shared/models/iAM/network';
import NetworkService from '../services/network.service';
import {clone} from 'ramda';
import {AxiosResponse} from 'axios';
import {http2XX, setStatusMessage} from '@/shared/utils/http-utils';

const state = {
    networks: [] as Network[]
};

const mutations = {
    networksMutator(state: any, networks: Network[]) {
        state.networks = clone(networks);
    }
};

const actions = {
    async getNetworks({dispatch, commit}: any) {
        await NetworkService.getNetworks()
            .then((response: AxiosResponse<Network[]>) => {
                if (http2XX.test(response.status.toString())) {
                    commit('networksMutator', response.data);
                } else {
                    dispatch('setErrorMessage', `Failed to get networks${setStatusMessage(response)}`);
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
