import {emptyTreatment, emptyTreatmentLibrary, Treatment, TreatmentLibrary} from '@/shared/models/iAM/treatment';
import {any, propEq, findIndex, clone, append} from 'ramda';
import TreatmentEditorService from '@/services/treatment-editor.service';

const state = {
    treatmentLibraries: [] as TreatmentLibrary[],
    scenarioTreatmentLibrary: clone(emptyTreatmentLibrary) as TreatmentLibrary,
    selectedTreatmentLibrary: clone(emptyTreatmentLibrary) as TreatmentLibrary
};

const mutations = {
    treatmentLibrariesMutator(state: any, treatmentLibraries: TreatmentLibrary[]) {
        state.treatmentLibraries = clone(treatmentLibraries);
    },
    selectedTreatmentLibraryMutator(state: any, treatmentLibraryId: number) {
        if (any(propEq('id', treatmentLibraryId), state.treatmentLibraries)) {
            // find the existing treatment library in state.treatmentLibraries where the id matches treatmentLibraryId,
            // copy it, then update state.selectedTreatmentLibrary with the copy
            state.selectedTreatmentLibrary = clone(state.treatmentLibraries
                .find((treatmentLibrary: TreatmentLibrary) =>
                    treatmentLibrary.id === treatmentLibraryId
                ) as TreatmentLibrary);
        } else {
            // update state.selectedTreatmentLibrary with an empty treatment library object
            state.selectedTreatmentLibrary = clone(emptyTreatmentLibrary);
        }
    },
    updatedSelectedTreatmentLibraryMutator(state: any, updatedSelectedTreatmentLibrary: TreatmentLibrary) {
        // update state.selectedTreatmentLibrary with the updated selected treatment library
        state.selectedTreatmentLibrary = clone(updatedSelectedTreatmentLibrary);
    },
    createdTreatmentLibraryMutator(state: any, createdTreatmentLibrary: TreatmentLibrary) {
        // append the created treatment library to a copy of state.treatmentLibraries, then update state.treatmentLibraries
        // with the copy
        state.treatmentLibraries = append(createdTreatmentLibrary, state.treatmentLibraries);
    },
    updatedTreatmentLibraryMutator(state: any, updatedTreatmentLibrary: TreatmentLibrary) {
        if (any(propEq('id', updatedTreatmentLibrary.id), state.treatmentLibraries)) {
            // make a copy of state.treatmentLibraries
            const treatmentLibraries: TreatmentLibrary[] = clone(state.treatmentLibraries);
            // find the index of the existing treatment library in the copy that has a matching id with the updated
            // treatment library
            const index: number = findIndex(propEq('id', updatedTreatmentLibrary.id), treatmentLibraries);
            // update the copy at the specified index with the updated treatment library
            treatmentLibraries[index] = clone(updatedTreatmentLibrary);
            // update state.treatmentLibraries with the copy
            state.treatmentLibraries = treatmentLibraries;
        }
    },
    scenarioTreatmentLibraryMutator(state: any, scenarioTreatmentLibrary: TreatmentLibrary) {
        state.scenarioTreatmentLibrary = clone(scenarioTreatmentLibrary);
    }
};

const actions = {
    async getTreatmentLibraries({dispatch, commit}: any) {
        await new TreatmentEditorService().getTreatmentLibraries()
            .then((treatmentLibraries: TreatmentLibrary[]) =>
                commit('treatmentLibrariesMutator', treatmentLibraries)
            )
            .catch((error: string) => dispatch('setErrorMessage', {message: error}));
    },
    selectTreatmentLibrary({commit}: any, payload: any) {
        commit('selectedTreatmentLibraryMutator', payload.treatmentLibraryId);
    },
    updateSelectedTreatmentLibrary({commit}: any, payload: any) {
        commit('updatedSelectedTreatmentLibraryMutator', payload.updatedSelectedTreatmentLibrary);
    },
    async createTreatmentLibrary({dispatch, commit}: any, payload: any) {
        await new TreatmentEditorService().createTreatmentLibrary(payload.createdTreatmentLibrary)
            .then(() => {
                commit('createdTreatmentLibraryMutator', payload.createdTreatmentLibrary);
                commit('selectedTreatmentLibraryMutator', payload.createdTreatmentLibrary.id);
            })
            .catch((error: string) => dispatch('setErrorMessage', {message: error}));
    },
    async updateTreatmentLibrary({dispatch, commit}: any, payload: any) {
        await new TreatmentEditorService().updateTreatmentLibrary(payload.updatedTreatmentLibrary)
            .then(() => {
                commit('updatedTreatmentLibraryMutator', payload.updatedTreatmentLibrary);
                commit('selectedTreatmentLibraryMutator', payload.updatedTreatmentLibrary.id);
            })
            .catch((error: string) => dispatch('setErrorMessage', {message: error}));
    },
    async getScenarioTreatmentLibrary({dispatch, commit}: any, payload: any) {
        await new TreatmentEditorService().getScenarioTreatmentLibrary(payload.selectedScenarioId)
            .then((scenarioTreatmentLibrary: TreatmentLibrary) => {
                commit('scenarioTreatmentLibraryMutator', scenarioTreatmentLibrary);
                commit('updatedSelectedTreatmentLibraryMutator', scenarioTreatmentLibrary);
            })
            .catch((error: string) => dispatch('setErrorMessage', {message: error}));
    },
    async upsertScenarioTreatmentLibrary({dispatch, commit}: any, payload: any) {
        await new TreatmentEditorService().upsertScenarioTreatmentLibrary(payload.upsertedScenarioTreatmentLibrary)
            .then(() => {
                commit('scenarioTreatmentLibraryMutator', payload.upsertedScenarioTreatmentLibrary);
                commit('updatedSelectedTreatmentLibraryMutator', payload.upsertedScenarioTreatmentLibrary);
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
