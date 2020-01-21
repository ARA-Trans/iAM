import {emptyTargetLibrary, TargetLibrary} from '@/shared/models/iAM/target';
import {clone, any, propEq, append, findIndex, equals, update} from 'ramda';
import TargetService from '@/services/target.service';
import {AxiosResponse} from 'axios';
import {hasValue} from '@/shared/utils/has-value-util';
import {convertFromMongoToVue} from '@/shared/utils/mongo-model-conversion-utils';

const state = {
    targetLibraries: [] as TargetLibrary[],
    selectedTargetLibrary: clone(emptyTargetLibrary) as TargetLibrary,
    scenarioTargetLibrary: clone(emptyTargetLibrary) as TargetLibrary
};

const mutations = {
    targetLibrariesMutator(state: any, targetLibraries: TargetLibrary[]) {
        state.targetLibraries = clone(targetLibraries);
    },
    selectedTargetLibraryMutator(state: any, selectedTargetLibrary: TargetLibrary) {
        state.selectedTargetLibrary = clone(selectedTargetLibrary);
    },
    createdTargetLibraryMutator(state: any, createdTargetLibrary: TargetLibrary) {
        state.targetLibraries = append(createdTargetLibrary, state.targetLibraries);
    },
    updatedTargetLibraryMutator(state: any, updatedTargetLibrary: TargetLibrary) {
        if (any(propEq('id', updatedTargetLibrary.id), state.targetLibraries)) {
            state.targetLibraries = update(
                findIndex(propEq('id', updatedTargetLibrary.id), state.targetLibraries),
                updatedTargetLibrary,
                state.targetLibraries
            );
        }
    },
    scenarioTargetLibraryMutator(state: any, scenarioTargetLibrary: TargetLibrary) {
        state.scenarioTargetLibrary = clone(scenarioTargetLibrary);
    }
};

const actions = {
    selectTargetLibrary({commit}: any, payload: any) {
        commit('selectedTargetLibraryMutator', payload.selectedTargetLibrary);
    },
    async getTargetLibraries({commit}: any) {
        await TargetService.getTargetLibraries()
            .then((response: AxiosResponse<any[]>) => {
                if (hasValue(response, 'data')) {
                    const targetLibraries: TargetLibrary[] = response.data
                        .map((data: any) => convertFromMongoToVue(data));
                    commit('targetLibrariesMutator', targetLibraries);
                }
            });
    },
    async createTargetLibrary({dispatch, commit}: any, payload: any) {
        await TargetService.createTargetLibrary(payload.createdTargetLibrary)
            .then((response: AxiosResponse<any>) => {
                if (hasValue(response, 'data')) {
                    const createdTargetLibrary: TargetLibrary = convertFromMongoToVue(response.data);
                    commit('createdTargetLibraryMutator', createdTargetLibrary);
                    dispatch('setSuccessMessage', {message: 'Successfully created target library'});
                }
            });
    },
    async updateTargetLibrary({dispatch, commit}: any, payload: any) {
        await TargetService.updateTargetLibrary(payload.updatedTargetLibrary)
            .then((response: AxiosResponse<any>) => {
                if (hasValue(response, 'data')) {
                    const updatedTargetLibrary: TargetLibrary = convertFromMongoToVue(response.data);
                    commit('updatedTargetLibraryMutator', updatedTargetLibrary);
                    commit('selectedTargetLibraryMutator', updatedTargetLibrary);
                    dispatch('setSuccessMessage', {message: 'Successfully updated target library'});
                }
            });
    },
    async getScenarioTargetLibrary({commit}: any, payload: any) {
        if (payload.selectedScenarioId > 0) {
            await TargetService.getScenarioTargetLibrary(payload.selectedScenarioId)
                .then((response: AxiosResponse<TargetLibrary>) => {
                    if (hasValue(response, 'data')) {
                        commit('scenarioTargetLibraryMutator', response.data);
                        commit('selectedTargetLibraryMutator', response.data);
                    }
                });
        }
    },
    async saveScenarioTargetLibrary({dispatch, commit}: any, payload: any) {
        await TargetService.saveScenarioTargetLibrary(payload.saveScenarioTargetLibraryData)
            .then((response: AxiosResponse<TargetLibrary>) => {
                if (hasValue(response, 'data')) {
                    commit('scenarioTargetLibraryMutator', response.data);
                    dispatch('setSuccessMessage', {message: 'Successfully saved scenario target library'});
                }
            });
    },
    async socket_targetLibrary({dispatch, state, commit}: any, payload: any) {
        if (hasValue(payload, 'operationType') && hasValue(payload, 'fullDocument')) {
            const targetLibrary: TargetLibrary = convertFromMongoToVue(payload.fullDocument);
            switch (payload.operationType) {
                case 'update':
                case 'replace':
                    commit('updatedTargetLibraryMutator', targetLibrary);
                    if (state.selectedTargetLibrary.id === targetLibrary.id &&
                        !equals(state.selectedTargetLibrary, targetLibrary)) {
                        commit('selectedTargetLibrary', targetLibrary);
                        dispatch('setInfoMessage',
                            {message: `Target library '${targetLibrary.name}' has been changed from another source`}
                        );
                    }
                    break;
                case 'insert':
                    if (!any(propEq('id', targetLibrary.id), state.targetLibraries)) {
                        commit('createdTargetLibraryMutator', targetLibrary);
                        dispatch('setInfoMessage',
                            {message: `Target library '${targetLibrary.name}' has been created from another source`}
                        );
                    }
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
