import {emptyTreatmentLibrary, Treatment, TreatmentLibrary} from '@/shared/models/iAM/treatment';
import {any, propEq, findIndex, clone, append} from 'ramda';
import TreatmentEditorService from '@/services/treatment-editor.service';
import {AxiosResponse} from 'axios';
import {http2XX, setStatusMessage} from '@/shared/utils/http-utils';
import {hasValue} from '@/shared/utils/has-value-util';

const convertFromMongoToVueModel = (data: any) => {
    const treatmentLibrary: any = {
        ...data,
        id: data._id,
        treatments: data.treatments
            .map((treatment: any) => {
                const subData: any = {
                    ...treatment,
                    id: treatment._id,
                    feasibility: {
                        ...treatment.feasibility,
                        id: treatment.feasibility._id
                    },
                    costs: treatment.costs.map((cost: any) => {
                        const childSubData: any = {
                            ...cost,
                            id: cost._id
                        };
                        delete childSubData._id;
                        delete childSubData.__v;
                        return childSubData;
                    }),
                    consequences: treatment.consequences.map((consequence: any) => {
                        const childSubData: any = {
                            ...consequence,
                            id: consequence._id
                        };
                        delete childSubData._id;
                        delete childSubData.__v;
                        return childSubData;
                    })
                };
                delete subData._id;
                delete subData.__v;
                delete subData.feasibility._id;
                delete subData.feasibility.__v;
                return subData as Treatment;
            })
    };
    delete treatmentLibrary._id;
    delete treatmentLibrary.__v;
    return treatmentLibrary as TreatmentLibrary;
};

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
            .then((response: AxiosResponse<any[]>) => {
                if (hasValue(response) && http2XX.test(response.status.toString())) {
                    const treatmentLibraries: TreatmentLibrary[] = response.data.map((data: any) => {
                        return convertFromMongoToVueModel(data);
                    });
                    commit('treatmentLibrariesMutator', treatmentLibraries);
                } else {
                    dispatch('setErrorMessage', {message: `Failed to get treatment libraries${setStatusMessage(response)}`});
                }
            });
    },
    async createTreatmentLibrary({dispatch, commit}: any, payload: any) {
        await TreatmentEditorService.createTreatmentLibrary(payload.createdTreatmentLibrary)
            .then((response: AxiosResponse<any>) => {
                if (hasValue(response) && http2XX.test(response.status.toString())) {
                    const createdTreatmentLibrary: TreatmentLibrary = convertFromMongoToVueModel(response.data);
                    commit('createdTreatmentLibraryMutator', createdTreatmentLibrary);
                    commit('selectedTreatmentLibraryMutator', createdTreatmentLibrary.id);
                    dispatch('setSuccessMessage', {message: 'Successfully created treatment library'});
                } else {
                    dispatch('setErrorMessage', {message: `Failed to create treatment library${setStatusMessage(response)}`});
                }
            });
    },
    async updateTreatmentLibrary({dispatch, commit}: any, payload: any) {
        await TreatmentEditorService.updateTreatmentLibrary(payload.updatedTreatmentLibrary)
            .then((response: AxiosResponse<any>) => {
                if (hasValue(response) && http2XX.test(response.status.toString())) {
                    const updatedTreatmentLibrary: TreatmentLibrary = convertFromMongoToVueModel(response.data);
                    commit('updatedTreatmentLibraryMutator', updatedTreatmentLibrary);
                    commit('selectedTreatmentLibraryMutator', updatedTreatmentLibrary.id);
                    dispatch('setSuccessMessage', {message: 'Successfully updated treatment library'});
                } else {
                    dispatch('setErrorMessage', {message: `Failed to update treatment library${setStatusMessage(response)}`});
                }
            });
    },
    async getScenarioTreatmentLibrary({dispatch, commit}: any, payload: any) {
        await TreatmentEditorService.getScenarioTreatmentLibrary(payload.selectedScenarioId)
            .then((response: AxiosResponse<TreatmentLibrary>) => {
                if (hasValue(response) && http2XX.test(response.status.toString())) {
                    commit('scenarioTreatmentLibraryMutator', response.data);
                    commit('updatedSelectedTreatmentLibraryMutator', response.data);
                } else {
                    dispatch('setErrorMessage', {message: `Failed to get scenario treatment library${setStatusMessage(response)}`});
                }
            });
    },
    async saveScenarioTreatmentLibrary({dispatch, commit}: any, payload: any) {
        await TreatmentEditorService.saveScenarioTreatmentLibrary(payload.saveScenarioTreatmentLibraryData)
            .then((response: AxiosResponse<TreatmentLibrary>) => {
                if (hasValue(response) && http2XX.test(response.status.toString())) {
                    commit('scenarioTreatmentLibraryMutator', response.data);
                    commit('updatedSelectedTreatmentLibraryMutator', response.data);
                    dispatch('setSuccessMessage', {message: 'Successfully saved scenario treatment library'});
                } else {
                    dispatch('setErrorMessage', {message: `Failed to update scenario treatment library${setStatusMessage(response)}`});
                }
            });
    },
    async socket_treatmentLibrary({dispatch, state, commit}: any, payload: any) {
        if (payload.operationType == 'update' || payload.operationType == 'replace') {
            const updatedTreatmentLibrary: TreatmentLibrary = convertFromMongoToVueModel(payload.fullDocument);
            commit('updatedTreatmentLibraryMutator', updatedTreatmentLibrary);
            if (state.selectedTreatmentLibrary.id == updatedTreatmentLibrary.id) {
                commit('selectedTreatmentLibraryMutator', updatedTreatmentLibrary.id);
            }
            dispatch('setInfoMessage', {message: 'Library data has been changed from another source'});
        }
        if (payload.operationType == 'insert') {
            const createdTreatmentLibrary: TreatmentLibrary = convertFromMongoToVueModel(payload.fullDocument);
            commit('createdTreatmentLibraryMutator', createdTreatmentLibrary);
            dispatch('setInfoMessage', {message: 'A new library has been created from another source'});
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
