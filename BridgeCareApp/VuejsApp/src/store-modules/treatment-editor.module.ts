import {emptyTreatment, emptyTreatmentLibrary, Treatment, TreatmentLibrary} from '@/shared/models/iAM/treatment';
import {any, propEq, findIndex, clone, append} from 'ramda';
import TreatmentEditorService from '@/services/treatment-editor.service';
import {AxiosResponse} from 'axios';
import {http2XX, setStatusMessage} from '@/shared/utils/http-utils';

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
    selectTreatmentLibrary({commit}: any, payload: any) {
        commit('selectedTreatmentLibraryMutator', payload.treatmentLibraryId);
    },
    updateSelectedTreatmentLibrary({commit}: any, payload: any) {
        commit('updatedSelectedTreatmentLibraryMutator', payload.updatedSelectedTreatmentLibrary);
    },
    async getTreatmentLibraries({dispatch, commit}: any) {
        await TreatmentEditorService.getTreatmentLibraries()
            .then((response: AxiosResponse<TreatmentLibrary[]>) => {
                if (http2XX.test(response.status.toString())) {
                    commit('treatmentLibrariesMutator', response.data);
                } else {
                    dispatch('setErrorMessage', {message: `Failed to get treatment libraries${setStatusMessage(response)}`});
                }
            });
    },
    async createTreatmentLibrary({dispatch, commit}: any, payload: any) {
        await TreatmentEditorService.createTreatmentLibrary(payload.createdTreatmentLibrary)
            .then((response: AxiosResponse<TreatmentLibrary>) => {
                if (http2XX.test(response.status.toString())) {
                    commit('createdTreatmentLibraryMutator', response.data);
                    commit('selectedTreatmentLibraryMutator', response.data.id);
                    dispatch('setSuccessMessage', {message: 'Successfully created treatment library'});
                } else {
                    dispatch('setErrorMessage', {message: `Failed to create treatment library${setStatusMessage(response)}`});
                }
            });
    },
    async updateTreatmentLibrary({dispatch, commit}: any, payload: any) {
        await TreatmentEditorService.updateTreatmentLibrary(payload.updatedTreatmentLibrary)
            .then((response: AxiosResponse<TreatmentLibrary>) => {
                if (http2XX.test(response.status.toString())) {
                    commit('updatedTreatmentLibraryMutator', response.data);
                    commit('selectedTreatmentLibraryMutator', response.data.id);
                    dispatch('setSuccessMessage', {message: 'Successfully updated treatment library'});
                } else {
                    dispatch('setErrorMessage', {message: `Failed to update treatment library${setStatusMessage(response)}`});
                }
            });
    },
    async getScenarioTreatmentLibrary({dispatch, commit}: any, payload: any) {
        await TreatmentEditorService.getScenarioTreatmentLibrary(payload.selectedScenarioId)
            .then((response: AxiosResponse<TreatmentLibrary>) => {
                if (http2XX.test(response.status.toString())) {
                    commit('scenarioTreatmentLibraryMutator', response.data);
                    commit('updatedSelectedTreatmentLibraryMutator', response.data);
                } else {
                    dispatch('setErrorMessage', {message: `Failed to get scenario treatment library${setStatusMessage(response)}`});
                }
            });
    },
    async saveScenarioTreatmentLibrary({dispatch, commit}: any, payload: any) {
        await TreatmentEditorService.saveScenarioTreatmentLibrary(payload.upsertedScenarioTreatmentLibrary)
            .then((response: AxiosResponse<TreatmentLibrary>) => {
                if (http2XX.test(response.status.toString())) {
                    commit('scenarioTreatmentLibraryMutator', response.data);
                    commit('updatedSelectedTreatmentLibraryMutator', response.data);
                    dispatch('setSuccessMessage', {message: 'Successfully saved scenario treatment library'});
                } else {
                    dispatch('setErrorMessage', {message: `Failed to update scenario treatment library${setStatusMessage(response)}`});
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
