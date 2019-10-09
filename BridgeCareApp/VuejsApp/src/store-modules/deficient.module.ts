import {Deficient, DeficientLibrary, emptyDeficientLibrary} from '@/shared/models/iAM/deficient';
import {clone} from 'ramda';
import DeficientService from '@/services/deficient.service';
import {AxiosResponse} from 'axios';
import {hasValue} from '@/shared/utils/has-value-util';

const state = {
    scenarioDeficientLibrary: clone(emptyDeficientLibrary) as DeficientLibrary
};

const mutations = {
    scenarioDeficientLibraryMutator(state: any, scenarioDeficientLibrary: DeficientLibrary) {
        state.scenarioDeficientLibrary = clone(scenarioDeficientLibrary);
    }
};

const actions = {
    async getScenarioDeficientLibrary({commit}: any, payload: any) {
        await DeficientService.getScenarioDeficientLibrary(payload.selectedScenarioId)
            .then((response: AxiosResponse<Deficient[]>) => {
                if (hasValue(response, 'data')) {
                    commit('scenarioDeficientLibraryMutator', response.data);
                }
            });
    },
    async saveScenarioDeficientLibrary({dispatch, commit}: any, payload: any) {
        await DeficientService.saveScenarioDeficientLibrary(payload.scenarioDeficientLibraryData)
            .then((response: AxiosResponse<Deficient[]>) => {
                if (hasValue(response, 'data')) {
                    commit('scenarioDeficientLibraryMutator', response.data);
                    dispatch('setSuccessMessage', {message: 'Successfully saved scenario deficient library'});
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
