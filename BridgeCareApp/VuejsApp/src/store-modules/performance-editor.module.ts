import {emptyPerformanceLibrary, PerformanceLibrary} from '@/shared/models/iAM/performance';
import {clone, append, any, propEq, findIndex} from 'ramda';
import PerformanceEditorService from '@/services/performance-editor.service';
import {AxiosResponse} from 'axios';
import {http2XX, setStatusMessage} from '@/shared/utils/http-utils';

const state = {
    performanceLibraries: [] as PerformanceLibrary[],
    scenarioPerformanceLibrary: clone(emptyPerformanceLibrary) as PerformanceLibrary,
    selectedPerformanceLibrary: clone(emptyPerformanceLibrary) as PerformanceLibrary
};

const mutations = {
    performanceLibrariesMutator(state: any, performanceLibraries: PerformanceLibrary[]) {
        // update state.performanceLibraries with a clone of the incoming list of performance libraries
        state.performanceLibraries = clone(performanceLibraries);
    },
    selectedPerformanceLibraryMutator(state: any, performanceLibraryId: number) {
        if (any(propEq('id', performanceLibraryId), state.performanceLibraries)) {
            // find the existing performance library in state.performanceLibraries where the id matches performanceLibraryId,
            // clone it, then update state.selectedPerformanceLibrary with the cloned, existing performance library
            state.selectedPerformanceLibrary = clone(state.performanceLibraries
                .find((performanceLibrary: PerformanceLibrary) =>
                    performanceLibrary.id === performanceLibraryId
                ) as PerformanceLibrary);
        } else {
            // reset state.selectedPerformanceLibrary as an empty performance library
            state.selectedPerformanceLibrary = clone(emptyPerformanceLibrary);
        }
    },
    updatedSelectedPerformanceLibraryMutator(state: any, updatedSelectedPerformanceLibrary: PerformanceLibrary) {
        // update the state.selectedPerformanceLibrary with the updated selected performance library
        state.selectedPerformanceLibrary = clone(updatedSelectedPerformanceLibrary);
    },
    createdPerformanceLibraryMutator(state: any, createdPerformanceLibrary: PerformanceLibrary) {
        // append the created performance library to a cloned list of state.performanceLibraries, then update
        // state.performanceLibraries with the cloned list
        state.performanceLibraries = append(createdPerformanceLibrary, state.performanceLibraries);
    },
    updatedPerformanceLibraryMutator(state: any, updatedPerformanceLibrary: PerformanceLibrary) {
        if (any(propEq('id', updatedPerformanceLibrary.id), state.performanceLibraries)) {
            // clone the list of performance libraries in state
            const performanceLibraries: PerformanceLibrary[] = clone(state.performanceLibraries);
            // find the index of the existing performance library in the cloned list of performance libraries that has
            // a matching id with the updated performance library
            const index: number = findIndex(propEq('id', updatedPerformanceLibrary.id), performanceLibraries);
            // set the updated performance library at the specified index
            performanceLibraries[index] = clone(updatedPerformanceLibrary);
            // update state.performanceLibraries with the cloned list of performance libraries
            state.performanceLibraries = performanceLibraries;
        }
    },
    scenarioPerformanceLibraryMutator(state: any, scenarioPerformanceLibrary: PerformanceLibrary) {
        state.scenarioPerformanceLibrary = clone(scenarioPerformanceLibrary);
    }
};

const actions = {
    selectPerformanceLibrary({commit}: any, payload: any) {
        commit('selectedPerformanceLibraryMutator', payload.performanceLibraryId);
    },
    updateSelectedPerformanceLibrary({commit}: any, payload: any) {
        commit('updatedSelectedPerformanceLibraryMutator', payload.updatedSelectedPerformanceLibrary);
    },
    async getPerformanceLibraries({dispatch, commit}: any) {
        await PerformanceEditorService.getPerformanceLibraries()
            .then((response: AxiosResponse<PerformanceLibrary[]>) => {
                if (http2XX.test(response.status.toString())) {
                    commit('performanceLibrariesMutator', response.data);
                } else {
                    dispatch('setErrorMessage', {message: `Failed to get performance libraries${setStatusMessage(response)}`});
                }
            });
    },
    async createPerformanceLibrary({dispatch, commit}: any, payload: any) {
        await PerformanceEditorService.createPerformanceLibrary(payload.createdPerformanceLibrary)
            .then((response: AxiosResponse<PerformanceLibrary>) => {
                if (http2XX.test(response.status.toString())) {
                    commit('createdPerformanceLibraryMutator', response.data);
                    commit('selectedPerformanceLibraryMutator', response.data.id);
                    dispatch('setSuccessMessage', {message: 'Successfully created performance library'});
                } else {
                    dispatch('setErrorMessage', {message: `Failed to create performance library${setStatusMessage(response)}`});
                }
            });
    },
    async updatePerformanceLibrary({dispatch, commit}: any, payload: any) {
        await PerformanceEditorService.updatePerformanceLibrary(payload.updatedPerformanceLibrary)
            .then((response: AxiosResponse<PerformanceLibrary>) => {
                if (http2XX.test(response.status.toString())) {
                    commit('updatedPerformanceLibraryMutator', response.data);
                    commit('selectedPerformanceLibraryMutator', response.data.id);
                    dispatch('setSuccessMessage', {message: 'Successfully updated performance library'});
                } else {
                    dispatch('setErrorMessage', {message: `Failed to update performance library${setStatusMessage(response)}`});
                }
            });
    },
    async getScenarioPerformanceLibrary({dispatch, commit}: any, payload: any) {
        await PerformanceEditorService.getScenarioPerformanceLibrary(payload.selectedScenarioId)
            .then((response: AxiosResponse<PerformanceLibrary>) => {
                if (http2XX.test(response.status.toString())) {
                    commit('scenarioPerformanceLibraryMutator', response.data);
                    commit('updatedSelectedPerformanceLibraryMutator', response.data);
                } else {
                    dispatch('setErrorMessage', {message: `Failed to get scenario performance library${setStatusMessage(response)}`});
                }
            });
    },
    async saveScenarioPerformanceLibrary({dispatch, commit}: any, payload: any) {
        await PerformanceEditorService.saveScenarioPerformanceLibrary(payload.saveScenarioPerformanceLibraryData)
            .then((response: AxiosResponse<PerformanceLibrary>) => {
                if (http2XX.test(response.status.toString())) {
                    commit('scenarioPerformanceLibraryMutator', response.data);
                    commit('updatedSelectedPerformanceLibraryMutator', response.data);
                    dispatch('setSuccessMessage', {message: 'Successfully saved scenario performance library'});
                } else {
                    dispatch('setErrorMessage', {message: `Failed to save scenario performance library${setStatusMessage(response)}`});
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
