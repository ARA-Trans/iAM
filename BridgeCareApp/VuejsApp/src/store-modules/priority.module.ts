import {Priority} from '@/shared/models/iAM/priority';
import {clone} from 'ramda';
import PriorityService from '@/services/priority.service';
import {AxiosResponse} from 'axios';

const state = {
    priorities: [] as Priority[]
};

const mutations = {
    prioritiesMutator(state: any, priorities: Priority[]) {
        state.priorities = clone(priorities);
    }
};

const actions = {
    async getPriorities({commit}: any, payload: any) {
        await PriorityService.getPriorities(payload.selectedScenarioId)
            .then((response: AxiosResponse<Priority[]>) =>
                commit('prioritiesMutator', response.data)
            );
    },
    async savePriorities({dispatch, commit}: any, payload: any) {
        await PriorityService.savePriorities(payload.selectedScenarioId, payload.priorityData)
            .then((response: AxiosResponse<Priority[]>) => {
                commit('prioritiesMutator', response.data);
                dispatch('setSuccessMessage', {message: 'Priority data successfully saved'});
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
