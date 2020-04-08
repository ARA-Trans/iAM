import {CriteriaLibrary, emptyCriteriaLibrary} from '@/shared/models/iAM/criteria';
import {any, append, clone, equals, find, findIndex, propEq, update} from 'ramda';
import CriteriaEditorService from '@/services/criteria-editor.service';
import {AxiosResponse} from 'axios';
import {hasValue} from '@/shared/utils/has-value-util';
import {convertFromMongoToVue} from '@/shared/utils/mongo-model-conversion-utils';

const state = {
    criteriaLibraries: [] as CriteriaLibrary[],
    selectedCriteriaLibrary: clone(emptyCriteriaLibrary) as CriteriaLibrary
};

const mutations = {
    criteriaLibrariesMutator(state: any, criteriaLibraries: CriteriaLibrary[]) {
        state.criteriaLibraries = clone(criteriaLibraries);
    },
    selectedCriteriaLibraryMutator(state: any, libraryId: string) {
        if (any(propEq('id', libraryId), state.criteriaLibraries)) {
            state.selectedCriteriaLibrary = find(propEq('id', libraryId), state.criteriaLibraries);
        } else {
            state.selectedCriteriaLibrary = clone(emptyCriteriaLibrary);
        }
    },
    createdCriteriaLibraryMutator(state: any, createdCriteriaLibrary: CriteriaLibrary) {
        state.criteriaLibraries = append(createdCriteriaLibrary, state.criteriaLibraries);
    },
    updatedCriteriaLibraryMutator(state: any, updatedCriteriaLibrary: CriteriaLibrary) {
        state.criteriaLibraries = update(
            findIndex(propEq('id', updatedCriteriaLibrary.id), state.criteriaLibraries),
            updatedCriteriaLibrary,
            state.criteriaLibraries
        );
    },
    deletedCriteriaLibraryMutator(state: any, deletedCriteriaLibraryId: string) {

    }
};

const actions = {
    selectCriteriaLibrary({commit}: any, payload: any) {
        commit('selectedCriteriaLibraryMutator', payload.selectedLibraryId);
    },
    async getCriteriaLibraries({commit}: any) {
        await CriteriaEditorService.getCriteriaLibraries()
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    const criteriaLibraries: CriteriaLibrary[] = response.data.map((data: any) => {
                        convertFromMongoToVue(data);
                    });
                    commit('criteriaLibrariesMutator', criteriaLibraries);
                }
            });
    },
    async createCriteriaLibrary({commit, dispatch}: any, payload: any) {
        await CriteriaEditorService.createCriteriaLibrary(payload.newCriteriaLibrary)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    const createdCriteriaLibrary: CriteriaLibrary = convertFromMongoToVue(response.data);
                    commit('createdCriteriaLibraryMutator', createdCriteriaLibrary);
                    dispatch('setSuccessMessage', {message: 'Successfully created criteria library'});
                }
            });
    },
    async updateCriteriaLibrary({commit, dispatch}: any, payload: any) {
        await CriteriaEditorService.updateCriteriaLibrary(payload.modifiedCriteriaLibrary)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    const updatedCriteriaLibrary: CriteriaLibrary = convertFromMongoToVue(response.data);
                    commit('updatedCriteriaLibraryMutator', updatedCriteriaLibrary);
                    dispatch('setSuccessMessage', {message: 'Successfully updated criteria library'});
                }
            });
    },
    async deleteCriteriaLibrary({commit, dispatch}: any, payload: any) {
        await CriteriaEditorService.deleteCriteriaLibrary(payload.criteriaLibraryId)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    commit('deletedCriteriaLibraryMutator', payload.criteriaLibraryId);
                    dispatch('setSuccessMessage', {message: 'Successfully deleted criteria library'});
                }
            });
    },
    async socket_criteriaLibrary({dispatch, state, commit}: any, payload: any) {
        if (hasValue(payload, 'operationType')) {
            if (hasValue(payload, 'fullDocument')) {
                const criteriaLibrary: CriteriaLibrary = convertFromMongoToVue(payload.fullDocument);
                switch (payload.operationType) {
                    case 'update':
                    case 'replace':
                        commit('updatedCriteriaLibraryMutator', criteriaLibrary);
                        if (state.selectedCriteriaLibrary.id === criteriaLibrary.id &&
                            !equals(state.selectedCriteriaLibrary, criteriaLibrary)) {

                        }
                        break;
                    case 'insert':
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
