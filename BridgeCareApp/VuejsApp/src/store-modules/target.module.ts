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
    async saveTargets({commit}: any, payload: any) {
        await TargetService.saveTargets(payload.targets)
            .then((response: AxiosResponse<Target[]>) =>
                commit('targetsMutator', response.data)
            );
    }
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations
};
