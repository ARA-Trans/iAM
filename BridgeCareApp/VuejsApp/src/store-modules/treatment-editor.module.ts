import {emptyTreatmentLibrary, TreatmentLibrary} from '@/shared/models/iAM/treatment';
import {any, propEq, findIndex} from 'ramda';
import TreatmentEditorService from '@/services/treatment-editor.service';

const state = {
    treatmentLibraries: [] as TreatmentLibrary[],
    scenarioTreatmentLibraries: [] as TreatmentLibrary[],
    selectedTreatmentLibrary: {...emptyTreatmentLibrary} as TreatmentLibrary
};

const mutations = {
    treatmentLibrariesMutator(state: any, treatmentLibraries: TreatmentLibrary[]) {
        state.treatmentLibraries = [...treatmentLibraries];
    },
    selectedTreatmentLibraryMutator(state: any, treatmentLibraryId: number) {
        if (any(propEq('id', treatmentLibraryId), state.treatmentLibraries)) {
            // find the existing treatment library in state.treatmentLibraries where the id matches treatmentLibraryId,
            // copy it, then update state.selectedTreatmentLibrary with the copy
            state.selectedTreatmentLibrary = {...state.treatmentLibraries
                .find((treatmentLibrary: TreatmentLibrary) =>
                    treatmentLibrary.id === treatmentLibraryId
                ) as TreatmentLibrary};
        } else if (treatmentLibraryId === null) {
            // update state.selectedTreatmentLibrary with an empty treatment library object
            state.selectedTreatmentLibrary = {...emptyTreatmentLibrary};
        }
    },
    updatedSelectedTreatmentLibraryMutator(state: any, updatedSelectedTreatmentLibrary: TreatmentLibrary) {
        // update state.selectedTreatmentLibrary with the updated selected treatment library
        state.selectedTreatmentLibrary = {...updatedSelectedTreatmentLibrary};
    },
    createdTreatmentLibraryMutator(state: any, createdTreatmentLibrary: TreatmentLibrary) {
        // append the created treatment library to a copy of state.treatmentLibraries, then update state.treatmentLibraries
        // with the copy
        state.treatmentLibraries = [...state.treatmentLibraries, createdTreatmentLibrary];
    },
    updatedTreatmentLibraryMutator(state: any, updatedTreatmentLibrary: TreatmentLibrary) {
        if (any(propEq('id', updatedTreatmentLibrary.id), state.treatmentLibraries)) {
            // make a copy of state.treatmentLibraries
            const treatmentLibraries: TreatmentLibrary[] = [...state.treatmentLibraries];
            // find the index of the existing treatment library in the copy that has a matching id with the updated
            // treatment library
            const index: number = findIndex(propEq('id', updatedTreatmentLibrary.id), treatmentLibraries);
            // update the copy at the specified index with the updated treatment library
            treatmentLibraries[index] = updatedTreatmentLibrary;
            // update state.treatmentLibraries with the copy
            state.treatmentLibraries = treatmentLibraries;
        }
    }
};

const actions = {
    async getTreatmentLibraries({commit}: any) {
        await new TreatmentEditorService().getTreatmentLibraries()
            .then((treatmentLibraries: TreatmentLibrary[]) =>
                commit('treatmentLibrariesMutator', treatmentLibraries)
            )
            .catch((error: any) => console.log(error));
    },
    selectTreatmentLibrary({commit}: any, payload: any) {
        commit('selectedTreatmentLibraryMutator', payload.treatmentLibraryId);
    },
    updateSelectedTreatmentLibrary({commit}: any, payload: any) {
        commit('updatedSelectedTreatmentLibraryMutator', payload.updatedSelectedTreatmentLibrary);
    },
    async createTreatmentLibrary({commit}: any, payload: any) {
        await new TreatmentEditorService().createTreatmentLibrary(payload.createdTreatmentLibrary)
            .then((createdTreatmentLibrary: TreatmentLibrary) => {
                commit('createdTreatmentLibraryMutator', createdTreatmentLibrary);
                commit('selectedTreatmentLibraryMutator', createdTreatmentLibrary.id);
            })
            .catch((error: any) => console.log(error));
    },
    async updateTreatmentLibrary({commit}: any, payload: any) {
        await new TreatmentEditorService().updateTreatmentLibrary(payload.updatedTreatmentLibrary)
            .then((updatedTreatmentLibrary: TreatmentLibrary) => {
                commit('updatedTreatmentLibraryMutator', updatedTreatmentLibrary);
                commit('selectedTreatmentLibraryMutator', updatedTreatmentLibrary.id);
            })
            .catch((error: any) => console.log(error));
    }
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations
};
