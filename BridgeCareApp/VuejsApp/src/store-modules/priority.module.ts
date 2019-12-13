import {emptyPriorityLibrary, Priority, PriorityFund, PriorityLibrary} from '@/shared/models/iAM/priority';
import {clone, any, propEq, append, findIndex, equals, update, reject} from 'ramda';
import PriorityService from '@/services/priority.service';
import {AxiosResponse} from 'axios';
import {hasValue} from '@/shared/utils/has-value-util';
import { http2XX } from '@/shared/utils/http-utils';

const convertFromMongoToVueModel = (data: any) => {
    const priorityLibrary: any = {
        ...data,
        id: data._id,
        priorities: data.priorities.map((priority: any) => {
            const subData: any = {
                ...priority,
                id: priority._id,
                priorityFunds: priority.priorityFunds.map((priorityFund: any) => {
                    const deepData: any = {
                        ...priorityFund,
                        id: priorityFund._id
                    };
                    delete deepData._id;
                    delete deepData.__v;
                    return deepData as PriorityFund;
                })
            };
            delete subData._id;
            delete subData.__v;
            return subData as Priority;
        })
    };
    delete priorityLibrary._id;
    delete priorityLibrary.__v;
    return priorityLibrary as PriorityLibrary;
};

const state = {
    priorityLibraries: [] as PriorityLibrary[],
    selectedPriorityLibrary: clone(emptyPriorityLibrary) as PriorityLibrary,
    scenarioPriorityLibrary: clone(emptyPriorityLibrary) as PriorityLibrary
};

const mutations = {
    priorityLibrariesMutator(state: any, priorityLibraries: PriorityLibrary[]) {
        state.priorityLibraries = clone(priorityLibraries);
    },
    selectedPriorityLibraryMutator(state: any, selectedPriorityLibrary: PriorityLibrary) {
        state.selectedPriorityLibrary = clone(selectedPriorityLibrary);
    },
    createdPriorityLibraryMutator(state: any, createdPriorityLibrary: PriorityLibrary) {
        state.priorityLibraries = append(createdPriorityLibrary, state.priorityLibraries);
    },
    updatedPriorityLibraryMutator(state: any, updatedPriorityLibrary: PriorityLibrary) {
        if (any(propEq('id', updatedPriorityLibrary.id), state.priorityLibraries)) {
            state.priorityLibraries = update(
                findIndex(propEq('id', updatedPriorityLibrary.id), state.priorityLibraries),
                updatedPriorityLibrary,
                state.priorityLibraries
            );
        }
    },
    deletedPriorityLibraryMutator(state: any, deletedPriorityLibrary: PriorityLibrary) {
        if (any(propEq('id', deletedPriorityLibrary.id), state.priorityLibraries)) {
            state.priorityLibraries = reject(
                (library: PriorityLibrary) => deletedPriorityLibrary.id === library.id,
                state.priorityLibraries
            );
        }
    },
    scenarioPriorityLibraryMutator(state: any, scenarioPriorityLibrary: PriorityLibrary) {
        state.scenarioPriorityLibrary = clone(scenarioPriorityLibrary);
    }
};

const actions = {
    selectPriorityLibrary({commit}: any, payload: any) {
        commit('selectedPriorityLibraryMutator', payload.selectedPriorityLibrary);
    },
    async getPriorityLibraries({commit}: any) {
        await PriorityService.getPriorityLibraries()
            .then((response: AxiosResponse<any[]>) => {
                if (hasValue(response, 'data')) {
                    const priorityLibraries: PriorityLibrary[] = response.data.map((data: any) => {
                        return convertFromMongoToVueModel(data);
                    });
                    commit('priorityLibrariesMutator', priorityLibraries);
                }
            });
    },
    async createPriorityLibrary({dispatch, commit}: any, payload: any) {
        await PriorityService.createPriorityLibrary(payload.createdPriorityLibrary)
            .then((response: AxiosResponse<any>) => {
                if (hasValue(response, 'data')) {
                    const createdPriorityLibrary: PriorityLibrary = convertFromMongoToVueModel(response.data);
                    commit('createdPriorityLibraryMutator', createdPriorityLibrary);
                    commit('selectedPriorityLibraryMutator', createdPriorityLibrary);
                    dispatch('setSuccessMessage', {message: 'Successfully created priority library'});
                }
            });
    },
    async updatePriorityLibrary({dispatch, commit}: any, payload: any) {
        await PriorityService.updatePriorityLibrary(payload.updatedPriorityLibrary)
            .then((response: AxiosResponse<any>) => {
                if (hasValue(response, 'data')) {
                    const updatedPriorityLibrary: PriorityLibrary = convertFromMongoToVueModel(response.data);
                    commit('updatedPriorityLibraryMutator', updatedPriorityLibrary);
                    commit('selectedPriorityLibraryMutator', updatedPriorityLibrary);
                    dispatch('setSuccessMessage', {message: 'Successfully updated priority library'});
                }
            });
    },
    async deletePriorityLibrary({dispatch, commit}: any, payload: any) {
        await PriorityService.deletePriorityLibrary(payload.priorityLibrary)
            .then((response: AxiosResponse<any>) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                commit('deletedPriorityLibraryMutator', payload.priorityLibrary);
                dispatch('setSuccessMessage', {message: 'Successfully deleted priority library'});
                }
            });
    },
    async getScenarioPriorityLibrary({commit}: any, payload: any) {
        await PriorityService.getScenarioPriorityLibrary(payload.selectedScenarioId)
            .then((response: AxiosResponse<PriorityLibrary>) => {
                if (hasValue(response, 'data')) {
                    commit('scenarioPriorityLibraryMutator', response.data);
                    commit('selectedPriorityLibraryMutator', response.data);
                }
            });
    },
    async saveScenarioPriorityLibrary({dispatch, commit}: any, payload: any) {
        await PriorityService.saveScenarioPriorityLibrary(payload.saveScenarioPriorityLibraryData)
            .then((response: AxiosResponse<PriorityLibrary>) => {
                if (hasValue(response, 'data')) {
                    commit('scenarioPriorityLibraryMutator', response.data);
                    dispatch('setSuccessMessage', {message: 'Successfully saved scenario priority library'});
                }
            });
    },
    async socket_priorityLibrary({dispatch, state, commit}: any, payload: any) {
        if (hasValue(payload, 'operationType') && hasValue(payload, 'fullDocument')) {
            const priorityLibrary: PriorityLibrary = convertFromMongoToVueModel(payload.fullDocument);
            switch (payload.operationType) {
                case 'update':
                case 'replace':
                    commit('updatedPriorityLibraryMutator', priorityLibrary);
                    if (state.selectedPriorityLibrary.id === priorityLibrary.id &&
                        !equals(state.selectedPriorityLibrary, priorityLibrary)) {
                        commit('selectedPriorityLibraryMutator', priorityLibrary);
                        dispatch('setInfoMessage',
                            {message: `Priority library '${priorityLibrary.name}' has been changed from another source`}
                        );
                    }
                    break;
                case 'insert':
                    if (!any(propEq('id', priorityLibrary.id), state.priorityLibraries)) {
                        commit('createdPriorityLibraryMutator', priorityLibrary);
                        dispatch('setInfoMessage',
                            {message: ` Priority library '${priorityLibrary.name}' has been created from another source`}
                        );
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
