import {emptyTreatmentLibrary, TreatmentLibrary} from '@/shared/models/iAM/treatment';
import {any, append, clone, equals, find, findIndex, propEq, reject, update} from 'ramda';
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
    selectedTreatmentLibraryMutator(state: any, libraryId: string) {
        if (any(propEq('id', libraryId), state.treatmentLibraries)) {
            state.selectedTreatmentLibrary = find(propEq('id', libraryId), state.treatmentLibraries);
        } else if (state.scenarioTreatmentLibrary.id === libraryId) {
            state.selectedTreatmentLibrary = clone(state.scenarioTreatmentLibrary);
        } else {
            state.selectedTreatmentLibrary = clone(emptyTreatmentLibrary);
        }
    },
    createdTreatmentLibraryMutator(state: any, createdTreatmentLibrary: TreatmentLibrary) {
        state.treatmentLibraries = append(createdTreatmentLibrary, state.treatmentLibraries);
    },
    updatedTreatmentLibraryMutator(state: any, updatedTreatmentLibrary: TreatmentLibrary) {
        state.treatmentLibraries = update(
            findIndex(propEq('id', updatedTreatmentLibrary), state.treatmentLibraries),
            updatedTreatmentLibrary,
            state.treatmentLibraries
        );
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
        commit('selectedTreatmentLibraryMutator', payload.selectedLibraryId);
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
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    const createdTreatmentLibrary: TreatmentLibrary = convertFromMongoToVue(response.data);
                    commit('createdTreatmentLibraryMutator', createdTreatmentLibrary);
                    dispatch('setSuccessMessage', {message: 'Successfully created treatment library'});
                }
            });
    },
    async updateTreatmentLibrary({dispatch, commit}: any, payload: any) {
        await TreatmentEditorService.updateTreatmentLibrary(payload.updatedTreatmentLibrary)
            .then((response: AxiosResponse) => {
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
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                    commit('deletedTreatmentLibraryMutator', payload.treatmentLibrary.id);
                    dispatch('setSuccessMessage', {message: 'Successfully deleted treatment library'});
                }
            });
    },
    async getScenarioTreatmentLibrary({commit}: any, payload: any) {
        await TreatmentEditorService.getScenarioTreatmentLibrary(payload.selectedScenarioId)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    commit('scenarioTreatmentLibraryMutator', response.data);
                    commit('selectedTreatmentLibraryMutator', response.data.id);
                }
            });
    },
    async saveScenarioTreatmentLibrary({dispatch, commit}: any, payload: any) {
        await TreatmentEditorService.saveScenarioTreatmentLibrary(payload.saveScenarioTreatmentLibraryData, payload.objectIdMOngoDBForScenario)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    commit('scenarioTreatmentLibraryMutator', response.data);
                    dispatch('setSuccessMessage', {message: 'Successfully saved scenario treatment library'});
                }
            });
    },
    async socket_treatmentLibrary({dispatch, state, commit}: any, payload: any) {
        if (hasValue(payload, 'operationType')) {
            if (hasValue(payload, 'fullDocument')) {
                const treatmentLibrary: TreatmentLibrary = convertFromMongoToVue(payload.fullDocument);
                switch (payload.operationType) {
                    case 'update':
                    case 'replace':
                        commit('updatedTreatmentLibraryMutator', treatmentLibrary);
                        if (state.selectedTreatmentLibrary.id === treatmentLibrary.id &&
                            !equals(state.selectedTreatmentLibrary, treatmentLibrary)) {
                            commit('selectedTreatmentLibraryMutator', treatmentLibrary.id);
                            dispatch('setInfoMessage', {message: `Treatment library ${treatmentLibrary.name} has been changed from another source`});
                        }
                        break;
                    case 'insert':
                        if (!any(propEq('id', treatmentLibrary.id), state.treatmentLibraries)) {
                            commit('createdTreatmentLibraryMutator', treatmentLibrary);
                            dispatch('setInfoMessage', {message: `Treatment library ${treatmentLibrary.name} has been created from another source`});
                        }
                }
            } else if (hasValue(payload, 'documentKey')) {
                if (any(propEq('id', payload.documentKey._id), state.treatmentLibraries)) {
                    const deletedTreatmentLibrary: TreatmentLibrary = find(propEq('id', payload.documentKey._id), state.treatmentLibraries);
                    commit('deletedTreatmentLibraryMutator', payload.documentKey._id);

                    if (deletedTreatmentLibrary.id === state.selectedTreatmentLibrary.id) {
                        if (!equals(state.scenarioTreatmentLibrary, emptyTreatmentLibrary)) {
                            commit('selectedTreatmentLibraryMutator', state.scenarioTreatmentLibrary.id);
                        } else {
                            commit('selectedTreatmentLibraryMutator', null);
                        }
                    }

                    dispatch('setInfoMessage', {message: `Treatment library ${deletedTreatmentLibrary.name} has been deleted from another source`});
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
