import {emptyPerformanceLibrary, PerformanceLibrary} from '@/shared/models/iAM/performance';
import {clone, append, any, propEq, findIndex} from 'ramda';
import PerformanceEditorService from '@/services/performance-editor.service';
import {sortByProperty} from '@/shared/utils/sorter-utils';

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
    async getPerformanceLibraries({dispatch, commit}: any) {
        await new PerformanceEditorService().getPerformanceLibraries()
            .then((performanceLibraries: PerformanceLibrary[]) =>
                commit('performanceLibrariesMutator', performanceLibraries)
            )
            .catch((error: string) => dispatch('setErrorMessage', {message: error}));
    },
    selectPerformanceLibrary({commit}: any, payload: any) {
        commit('selectedPerformanceLibraryMutator', payload.performanceLibraryId);
    },
    updateSelectedPerformanceLibrary({commit}: any, payload: any) {
        commit('updatedSelectedPerformanceLibraryMutator', payload.updatedSelectedPerformanceLibrary);
    },
    async createPerformanceLibrary({dispatch, commit}: any, payload: any) {
        await new PerformanceEditorService().createPerformanceLibrary(payload.createdPerformanceLibrary)
            .then((createdPerformanceLibrary: PerformanceLibrary) => {
                commit('createdPerformanceLibraryMutator', createdPerformanceLibrary);
                commit('selectedPerformanceLibraryMutator', createdPerformanceLibrary.id);
            })
            .catch((error: string) => dispatch('setErrorMessage', {message: error}));
    },
    async updatePerformanceLibrary({dispatch, commit}: any, payload: any) {
        await new PerformanceEditorService().updatePerformanceLibrary(payload.updatedPerformanceLibrary)
            .then((updatedPerformanceLibrary: PerformanceLibrary) => {
                commit('updatedPerformanceLibraryMutator', updatedPerformanceLibrary);
                commit('selectedPerformanceLibraryMutator', updatedPerformanceLibrary.id);
            })
            .catch((error: string) => dispatch('setErrorMessage', {message: error}));
    },
    async getScenarioPerformanceLibrary({dispatch, commit}: any, payload: any) {
        await new PerformanceEditorService().getScenarioPerformanceLibrary(payload.selectedScenarioId)
            .then((scenarioPerformanceLibrary: PerformanceLibrary) => {
                commit('scenarioPerformanceLibraryMutator', scenarioPerformanceLibrary);
                commit('updatedSelectedPerformanceLibraryMutator', scenarioPerformanceLibrary);
            })
            .catch((error: string) => dispatch('setErrorMessage', {message: error}));
    },
    async upsertScenarioPerformanceLibrary({dispatch, commit}: any, payload: any) {
        await new PerformanceEditorService().upsertScenarioPerformanceLibrary(payload.upsertedScenarioPerformanceLibrary)
            .then((upsertedScenarioPerformanceLibrary: PerformanceLibrary) => {
                commit('scenarioPerformanceLibraryMutator', upsertedScenarioPerformanceLibrary);
                commit('updatedSelectedPerformanceLibraryMutator', upsertedScenarioPerformanceLibrary);
            })
            .catch((error: string) => dispatch('setErrorMessage', {message: error}));
    }
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations
};
