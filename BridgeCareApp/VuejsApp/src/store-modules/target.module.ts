import {emptyTargetLibrary, TargetLibrary} from '@/shared/models/iAM/target';
import {clone} from 'ramda';
import TargetService from '@/services/target.service';
import {AxiosResponse} from 'axios';
import {hasValue} from '@/shared/utils/has-value-util';

const state = {
    scenarioTargetLibrary: clone(emptyTargetLibrary) as TargetLibrary
};

const mutations = {
    scenarioTargetLibraryMutator(state: any, scenarioTargetLibrary: TargetLibrary) {
        // update state.scenarioTargetLibrary with a clone of the incoming scenario target library data
        state.scenarioTargetLibrary = clone(scenarioTargetLibrary);
    }
};

const actions = {
    async getScenarioTargetLibrary({commit}: any, payload: any) {
        if (payload.selectedScenarioId > 0) {
            await TargetService.getScenarioTargetLibrary(payload.selectedScenarioId)
                .then((response: AxiosResponse<TargetLibrary>) => {
                    if (hasValue(response, 'data')) {
                        commit('scenarioTargetLibraryMutator', response.data);
                    }
                });
        }
    },
    async saveScenarioTargetLibrary({dispatch, commit}: any, payload: any) {
        await TargetService.saveScenarioTargetLibrary(payload.saveScenarioTargetLibraryData)
            .then((response: AxiosResponse<TargetLibrary>) => {
                if (hasValue(response, 'data')) {
                    commit('targetsMutator', response.data);
                    dispatch('setSuccessMessage', {message: 'Successfully saved scenario target library'});
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
