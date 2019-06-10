import {Deficient} from '@/shared/models/iAM/deficient';
import {clone} from 'ramda';
import DeficientService from '@/services/deficient.service';
import {AxiosResponse} from 'axios';

const state = {
    deficients: [] as Deficient[]
};

const mutations = {
    deficientsMutator(state: any, deficients: Deficient[]) {
        state.deficients = clone(deficients);
    }
};

const actions = {
    async getDeficients({commit}: any, payload: any) {
        await DeficientService.getDeficients(payload.selectedScenarioId)
            .then((response: AxiosResponse<Deficient[]>) =>
                commit('deficientsMutator', response.data)
            );
    },
    async saveDeficients({dispatch, commit}: any, payload: any) {
        await DeficientService.saveDeficients(payload.deficientData)
            .then((response: AxiosResponse<Deficient[]>) => {
                commit('deficientsMutator', response.data);
                dispatch('setSuccessMessage', {message: 'Deficient data successfully saved'});
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
