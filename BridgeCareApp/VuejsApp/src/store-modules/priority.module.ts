import {emptyPriorityLibrary, PriorityLibrary} from '@/shared/models/iAM/priority';
import {any, append, clone, equals, find, findIndex, propEq, reject, update} from 'ramda';
import PriorityService from '@/services/priority.service';
import {AxiosResponse} from 'axios';
import {hasValue} from '@/shared/utils/has-value-util';
import {convertFromMongoToVue} from '@/shared/utils/mongo-model-conversion-utils';
import {http2XX} from '@/shared/utils/http-utils';

const state = {
    priorityLibraries: [] as PriorityLibrary[],
    selectedPriorityLibrary: clone(emptyPriorityLibrary) as PriorityLibrary,
    scenarioPriorityLibrary: clone(emptyPriorityLibrary) as PriorityLibrary
};

const mutations = {
    priorityLibrariesMutator(state: any, priorityLibraries: PriorityLibrary[]) {
        state.priorityLibraries = clone(priorityLibraries);
    },
    selectedPriorityLibraryMutator(state: any, libraryId: string) {
        if (any(propEq('id', libraryId), state.priorityLibraries)) {
            state.selectedPriorityLibrary = find(propEq('id', libraryId), state.priorityLibraries);
        } else if (state.scenarioPriorityLibrary.id === libraryId) {
            state.selectedPriorityLibrary = clone(state.scenarioPriorityLibrary);
        } else {
            state.selectedPriorityLibrary = clone(emptyPriorityLibrary);
        }
    },
    createdPriorityLibraryMutator(state: any, createdPriorityLibrary: PriorityLibrary) {
        state.priorityLibraries = append(createdPriorityLibrary, state.priorityLibraries);
    },
    updatedPriorityLibraryMutator(state: any, updatedPriorityLibrary: PriorityLibrary) {
        if (any(propEq('id', updatedPriorityLibrary.id), state.priorityLibraries)) {
            state.priorityLibraries = update(
                findIndex(propEq('id', updatedPriorityLibrary.id), state.priorityLibraries),
                updatedPriorityLibrary,
                state.priorityLibraries
            );
        }
    },
    deletedPriorityLibraryMutator(state: any, deletedPriorityLibraryId: string) {
        if (any(propEq('id', deletedPriorityLibraryId), state.priorityLibraries)) {
            state.priorityLibraries = reject(
                (library: PriorityLibrary) => deletedPriorityLibraryId === library.id,
                state.priorityLibraries
            );
        }
    },
    scenarioPriorityLibraryMutator(state: any, scenarioPriorityLibrary: PriorityLibrary) {
        state.scenarioPriorityLibrary = clone(scenarioPriorityLibrary);
    }
};

const actions = {
    selectPriorityLibrary({commit}: any, payload: any) {
        commit('selectedPriorityLibraryMutator', payload.selectedLibraryId);
    },
    async getPriorityLibraries({commit}: any) {
        await PriorityService.getPriorityLibraries()
            .then((response: AxiosResponse<any[]>) => {
                if (hasValue(response, 'data')) {
                    const priorityLibraries: PriorityLibrary[] = response.data
                        .map((data: any) => convertFromMongoToVue(data));
                    commit('priorityLibrariesMutator', priorityLibraries);
                }
            });
    },
    async createPriorityLibrary({dispatch, commit}: any, payload: any) {
        await PriorityService.createPriorityLibrary(payload.createdPriorityLibrary)
            .then((response: AxiosResponse<any>) => {
                if (hasValue(response, 'data')) {
                    const createdPriorityLibrary: PriorityLibrary = convertFromMongoToVue(response.data);
                    commit('createdPriorityLibraryMutator', createdPriorityLibrary);
                    dispatch('setSuccessMessage', {message: 'Successfully created priority library'});
                }
            });
    },
    async updatePriorityLibrary({dispatch, commit}: any, payload: any) {
        await PriorityService.updatePriorityLibrary(payload.updatedPriorityLibrary)
            .then((response: AxiosResponse<any>) => {
                if (hasValue(response, 'data')) {
                    const updatedPriorityLibrary: PriorityLibrary = convertFromMongoToVue(response.data);
                    commit('updatedPriorityLibraryMutator', updatedPriorityLibrary);
                    commit('selectedPriorityLibraryMutator', updatedPriorityLibrary.id);
                    dispatch('setSuccessMessage', {message: 'Successfully updated priority library'});
                }
            });
    },
    async deletePriorityLibrary({dispatch, commit}: any, payload: any) {
        await PriorityService.deletePriorityLibrary(payload.priorityLibrary)
            .then((response: AxiosResponse<any>) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                    commit('deletedPriorityLibraryMutator', payload.priorityLibrary.id);
                    dispatch('setSuccessMessage', {message: 'Successfully deleted priority library'});
                }
            });
    },
    async getScenarioPriorityLibrary({commit}: any, payload: any) {
        await PriorityService.getScenarioPriorityLibrary(payload.selectedScenarioId)
            .then((response: AxiosResponse<PriorityLibrary>) => {
                if (hasValue(response, 'data')) {
                    commit('scenarioPriorityLibraryMutator', response.data);
                    commit('selectedPriorityLibraryMutator', response.data.id);
                }
            });
    },
    async saveScenarioPriorityLibrary({dispatch, commit}: any, payload: any) {
        await PriorityService.saveScenarioPriorityLibrary(payload.saveScenarioPriorityLibraryData, payload.objectIdMOngoDBForScenario)
            .then((response: AxiosResponse<PriorityLibrary>) => {
                if (hasValue(response, 'data')) {
                    commit('scenarioPriorityLibraryMutator', response.data);
                    dispatch('setSuccessMessage', {message: 'Successfully saved scenario priority library'});
                }
            });
    },
    async socket_priorityLibrary({dispatch, state, commit}: any, payload: any) {
        if (hasValue(payload, 'operationType')) {
            if (hasValue(payload, 'fullDocument')) {
                const priorityLibrary: PriorityLibrary = convertFromMongoToVue(payload.fullDocument);
                switch (payload.operationType) {
                    case 'update':
                    case 'replace':
                        commit('updatedPriorityLibraryMutator', priorityLibrary);
                        if (state.selectedPriorityLibrary.id === priorityLibrary.id &&
                            !equals(state.selectedPriorityLibrary, priorityLibrary)) {
                            commit('selectedPriorityLibraryMutator', priorityLibrary.id);
                            dispatch('setInfoMessage',
                                {message: `Priority library '${priorityLibrary.name}' has been changed from another source`}
                            );
                        }
                        break;
                    case 'insert':
                        if (!any(propEq('id', priorityLibrary.id), state.priorityLibraries)) {
                            commit('createdPriorityLibraryMutator', priorityLibrary);
                            dispatch('setInfoMessage',
                                {message: `Priority library '${priorityLibrary.name}' has been created from another source`}
                            );
                        }
                }
            } else if (hasValue(payload, 'documentKey')) {
                if (any(propEq('id', payload.documentKey._id), state.priorityLibraries)) {
                    const deletedPriorityLibrary: PriorityLibrary = find(
                        propEq('id', payload.documentKey._id), state.priorityLibraries);
                    commit('deletedPriorityLibraryMutator', payload.documentKey._id);

                    if (deletedPriorityLibrary.id === state.selectedPriorityLibrary) {
                        if (!equals(state.scenarioPriorityLibrary, emptyPriorityLibrary)) {
                            commit('selectedPriorityLibraryMutator', state.scenarioPriorityLibrary.id);
                        } else {
                            commit('selectedPriorityLibraryMutator', null);
                        }
                    }

                    dispatch('setInfoMessage',
                        {message: `Priority library ${deletedPriorityLibrary.name} has been deleted from another source`}
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
