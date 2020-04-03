import {AxiosResponse} from 'axios';
import {any, clone, findIndex, propEq} from 'ramda';
import {hasValue} from '@/shared/utils/has-value-util';
import {http2XX} from '@/shared/utils/http-utils';
import prepend from 'ramda/es/prepend';
import {Rollup} from '@/shared/models/iAM/rollup';
import RollupService from '../services/rollup.service';
import {convertFromMongoToVue} from '@/shared/utils/mongo-model-conversion-utils';

const state = {
    rollups: [] as Rollup[],
};

const mutations = {
    rollupsMutator(state: any, rollup: Rollup[]) {
        state.rollups = clone(rollup);
    },
    createdNetworkMutator(state: any, createdNetwork: Rollup) {
        state.rollups = prepend(createdNetwork, state.rollups);
    },
    updatedRollupMutator(state: any, updatedRollup: Rollup) {
        if (any(propEq('id', updatedRollup.id), state.rollups)) {
            const rollups: Rollup[] = clone(state.rollups);
            const index: number = findIndex(propEq('id', updatedRollup.id), rollups);
            rollups[index] = clone(updatedRollup);
            state.rollups = rollups;
        }
    }
};

const actions = {
    async getMongoRollups({commit}: any) {
        return await RollupService.getMongoRollups()
            .then((response: AxiosResponse<Rollup[]>) => {
                const rollups: Rollup[] = response.data
                    .map((data: any) => convertFromMongoToVue(data));
                commit('rollupsMutator', rollups);
            });
    },
    async rollupNetwork({dispatch, commit}: any, payload: any) {
        await RollupService.rollupNetwork(payload.selectedNetwork)
            .then((response: AxiosResponse<any>) => {
                if (http2XX.test(response.status.toString())) {
                    dispatch('setSuccessMessage', {message: 'Rollup started'});
                }
            });
    },
    async getLegacyNetworks({commit}: any, payload: any) {
        return await RollupService.getLegacyNetworks(payload.networks)
            .then((response: AxiosResponse<Rollup[]>) => {
                if (hasValue(response)) {
                    const networks: Rollup[] = response.data
                        .map((data: any) => convertFromMongoToVue(data));
                    commit('rollupsMutator', networks);
                }
            });
    },
    async socket_rollupStatus({dispatch, state, commit}: any, payload: any) {
        if (payload.operationType == 'update' || payload.operationType == 'replace') {
            const updatedRollup: Rollup = convertFromMongoToVue(payload.fullDocument);
            commit('updatedRollupMutator', updatedRollup);
        }

        if (payload.operationType == 'insert') {
            const createdNetwork: Rollup = convertFromMongoToVue(payload.fullDocument);
            if (!any(propEq('id', createdNetwork.id), state.rollups)) {
                commit('createdNetworkMutator', createdNetwork);
                dispatch('setInfoMessage', {message: 'New network has been inserted from another source'});
            }
        }
    }
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations
};
