import {CriteriaLibrary, emptyCriteriaLibrary} from '@/shared/models/iAM/criteria';
import {any, clone, find, propEq} from 'ramda';
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
    }
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations
};
