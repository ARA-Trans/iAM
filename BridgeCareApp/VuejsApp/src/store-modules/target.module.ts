import {emptyTargetLibrary, TargetLibrary} from '@/shared/models/iAM/target';
import {any, append, clone, equals, find, findIndex, propEq, reject, update} from 'ramda';
import TargetService from '@/services/target.service';
import {AxiosResponse} from 'axios';
import {hasValue} from '@/shared/utils/has-value-util';
import {convertFromMongoToVue} from '@/shared/utils/mongo-model-conversion-utils';
import {http2XX} from '@/shared/utils/http-utils';

const state = {
    targetLibraries: [] as TargetLibrary[],
    selectedTargetLibrary: clone(emptyTargetLibrary) as TargetLibrary,
    scenarioTargetLibrary: clone(emptyTargetLibrary) as TargetLibrary
};

const mutations = {
    targetLibrariesMutator(state: any, targetLibraries: TargetLibrary[]) {
        state.targetLibraries = clone(targetLibraries);
    },
    selectedTargetLibraryMutator(state: any, libraryId: string) {
        if (any(propEq('id', libraryId), state.targetLibraries)) {
            state.selectedTargetLibrary = find(propEq('id', libraryId), state.targetLibraries);
        } else if (state.scenarioTargetLibrary.id === libraryId) {
            state.selectedTargetLibrary = clone(state.scenarioTargetLibrary);
        } else {
            state.selectedTargetLibrary = clone(emptyTargetLibrary);
        }
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
    deletedTargetLibraryMutator(state: any, deletedTargetLibraryId: string) {
        if (any(propEq('id', deletedTargetLibraryId), state.targetLibraries)) {
            state.targetLibraries = reject(
                (library: TargetLibrary) => deletedTargetLibraryId === library.id,
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
        commit('selectedTargetLibraryMutator', payload.selectedLibraryId);
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
                    commit('selectedTargetLibraryMutator', updatedTargetLibrary.id);
                    dispatch('setSuccessMessage', {message: 'Successfully updated target library'});
                }
            });
    },
    async deleteTargetLibrary({dispatch, commit}: any, payload: any) {
        await TargetService.deleteTargetLibrary(payload.targetLibrary)
            .then((response: AxiosResponse<any>) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                    commit('deletedTargetLibraryMutator', payload.targetLibrary.id);
                    dispatch('setSuccessMessage', {message: 'Successfully deleted target library'});
                }
            });
    },
    async getScenarioTargetLibrary({commit}: any, payload: any) {
        if (payload.selectedScenarioId > 0) {
            await TargetService.getScenarioTargetLibrary(payload.selectedScenarioId)
                .then((response: AxiosResponse<TargetLibrary>) => {
                    if (hasValue(response, 'data')) {
                        commit('scenarioTargetLibraryMutator', response.data);
                        commit('selectedTargetLibraryMutator', response.data.id);
                    }
                });
        }
    },
    async saveScenarioTargetLibrary({dispatch, commit}: any, payload: any) {
        await TargetService.saveScenarioTargetLibrary(payload.saveScenarioTargetLibraryData, payload.objectIdMOngoDBForScenario)
            .then((response: AxiosResponse<TargetLibrary>) => {
                if (hasValue(response, 'data')) {
                    commit('scenarioTargetLibraryMutator', response.data);
                    dispatch('setSuccessMessage', {message: 'Successfully saved scenario target library'});
                }
            });
    },
    async socket_targetLibrary({dispatch, state, commit}: any, payload: any) {
        if (hasValue(payload, 'operationType')) {
            if (hasValue(payload, 'fullDocument')) {
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
            } else if (hasValue(payload, 'documentKey')) {
                if (any(propEq('id', payload.documentKey._id), state.targetLibraries)) {
                    const deletedTargetLibrary: TargetLibrary = find(
                        propEq('id', payload.documentKey._id), state.targetLibraries);
                    commit('deletedTargetLibraryMutator', payload.documentKey._id);

                    if (deletedTargetLibrary.id === state.selectedTargetLibrary) {
                        if (!equals(state.scenarioTargetLibrary, emptyTargetLibrary)) {
                            commit('selectedTargetLibraryMutator', state.scenarioTargetLibrary.id);
                        } else {
                            commit('selectedTargetLibraryMutator', null);
                        }
                    }

                    dispatch('setInfoMessage',
                        {message: `Target library ${deletedTargetLibrary.name} has been deleted from another source`}
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
