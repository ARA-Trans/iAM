import {Target} from '@/shared/models/iAM/target';
import {clone} from 'ramda';
import TargetService from '@/services/target.service';
import {AxiosResponse} from 'axios';

const state = {
    targets: [] as Target[]
};

const mutations = {
    targetsMutator(state: any, targets: Target[]) {
        state.targets = clone(targets);
    }
};

const actions = {
    async getTargets({commit}: any, payload: any) {
        await TargetService.getTargets(payload.selectedScenarioId)
            .then((response: AxiosResponse<Target[]>) =>
                commit('targetsMutator', response.data)
            );
    },
    async saveTargets({dispatch, commit}: any, payload: any) {
        await TargetService.saveTargets(payload.targetData)
            .then((response: AxiosResponse<Target[]>) => {
                commit('targetsMutator', response.data);
                dispatch('setSuccessMessage', {message: 'Target data successfully saved'});
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
