import {emptyTreatmentLibrary, Treatment, TreatmentLibrary} from '@/shared/models/iAM/treatment';
import {any, propEq, findIndex, clone, append, equals, reject} from 'ramda';
import TreatmentEditorService from '@/services/treatment-editor.service';
import {AxiosResponse} from 'axios';
import {hasValue} from '@/shared/utils/has-value-util';
import {convertFromMongoToVue} from '@/shared/utils/mongo-model-conversion-utils';
import {http2XX} from '@/shared/utils/http-utils';

const state = {
    treatmentLibraries: [] as TreatmentLibrary[],
    scenarioTreatmentLibrary: clone(emptyTreatmentLibrary) as TreatmentLibrary,
    selectedTreatmentLibrary: clone(emptyTreatmentLibrary) as TreatmentLibrary
};

const mutations = {
    treatmentLibrariesMutator(state: any, treatmentLibraries: TreatmentLibrary[]) {
        state.treatmentLibraries = clone(treatmentLibraries);
    },
    selectedTreatmentLibraryMutator(state: any, treatmentLibraryId: string) {
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
    deletedTreatmentLibraryMutator(state: any, deletedTreatmentLibraryId: string) {
        if (any(propEq('id', deletedTreatmentLibraryId), state.treatmentLibraries)) {
            state.treatmentLibraries = reject(
                (library: TreatmentLibrary) => deletedTreatmentLibraryId === library.id,
                state.treatmentLibraries
            );
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
    async getTreatmentLibraries({commit}: any) {
        await TreatmentEditorService.getTreatmentLibraries()
            .then((response: AxiosResponse<any[]>) => {
                if (hasValue(response, 'data')) {
                    const treatmentLibraries: TreatmentLibrary[] = response.data
                        .map((data: any) => convertFromMongoToVue(data));
                    commit('treatmentLibrariesMutator', treatmentLibraries);
                }
            });
    },
    async createTreatmentLibrary({dispatch, commit}: any, payload: any) {
        await TreatmentEditorService.createTreatmentLibrary(payload.createdTreatmentLibrary)
            .then((response: AxiosResponse<any>) => {
                if (hasValue(response, 'data')) {
                    const createdTreatmentLibrary: TreatmentLibrary = convertFromMongoToVue(response.data);
                    commit('createdTreatmentLibraryMutator', createdTreatmentLibrary);
                    dispatch('setSuccessMessage', {message: 'Successfully created treatment library'});
                }
            });
    },
    async updateTreatmentLibrary({dispatch, commit}: any, payload: any) {
        await TreatmentEditorService.updateTreatmentLibrary(payload.updatedTreatmentLibrary)
            .then((response: AxiosResponse<any>) => {
                if (hasValue(response, 'data')) {
                    const updatedTreatmentLibrary: TreatmentLibrary = convertFromMongoToVue(response.data);
                    commit('updatedTreatmentLibraryMutator', updatedTreatmentLibrary);
                    commit('selectedTreatmentLibraryMutator', updatedTreatmentLibrary.id);
                    dispatch('setSuccessMessage', {message: 'Successfully updated treatment library'});
                }
            });
    },
    async deleteTreatmentLibrary({dispatch, commit}: any, payload: any) {
        await TreatmentEditorService.deleteTreatmentLibrary(payload.treatmentLibrary)
            .then((response: AxiosResponse<any>) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                commit('deletedTreatmentLibraryMutator', payload.treatmentLibrary.id);
                dispatch('setSuccessMessage', {message: 'Successfully deleted treatment library'});
                }
            });
    },
    async getScenarioTreatmentLibrary({commit}: any, payload: any) {
        await TreatmentEditorService.getScenarioTreatmentLibrary(payload.selectedScenarioId)
            .then((response: AxiosResponse<TreatmentLibrary>) => {
                if (hasValue(response, 'data')) {
                    commit('scenarioTreatmentLibraryMutator', response.data);
                    commit('updatedSelectedTreatmentLibraryMutator', response.data);
                }
            });
    },
    async saveScenarioTreatmentLibrary({dispatch, commit}: any, payload: any) {
        await TreatmentEditorService.saveScenarioTreatmentLibrary(payload.saveScenarioTreatmentLibraryData)
            .then((response: AxiosResponse<TreatmentLibrary>) => {
                if (hasValue(response, 'data')) {
                    commit('scenarioTreatmentLibraryMutator', response.data);
                    commit('updatedSelectedTreatmentLibraryMutator', response.data);
                    dispatch('setSuccessMessage', {message: 'Successfully saved scenario treatment library'});
                }
            });
    },
    async socket_treatmentLibrary({dispatch, state, commit}: any, payload: any) {
        if (hasValue(payload, 'operationType') && hasValue(payload, 'fullDocument')) {
            if (payload.operationType == 'update' || payload.operationType == 'replace') {
                const updatedTreatmentLibrary: TreatmentLibrary = convertFromMongoToVue(payload.fullDocument);
                commit('updatedTreatmentLibraryMutator', updatedTreatmentLibrary);

                if (state.selectedTreatmentLibrary.id === updatedTreatmentLibrary.id &&
                    !equals(state.selectedTreatmentLibrary, updatedTreatmentLibrary)) {
                    commit('selectedTreatmentLibraryMutator', updatedTreatmentLibrary.id);
                    dispatch('setInfoMessage', {message: 'Library data has been changed from another source'});
                }
            }

            if (payload.operationType == 'insert') {
                const createdTreatmentLibrary: TreatmentLibrary = convertFromMongoToVue(payload.fullDocument);
                commit('createdTreatmentLibraryMutator', createdTreatmentLibrary);
            }
        } else if (hasValue(payload, 'operationType') && payload.operationType === 'delete') {
            if (any(propEq('id', payload.documentKey._id), state.treatmentLibraries)) {
                commit('deletedTreatmentLibraryMutator', payload.documentKey._id);
                dispatch('setInfoMessage',
                    {message: `A treatment library has been deleted from another source`}
                );
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
