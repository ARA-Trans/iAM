import {emptyPriorityLibrary, Priority, PriorityFund, PriorityLibrary} from '@/shared/models/iAM/priority';
import {clone, any, propEq, append, findIndex, equals} from 'ramda';
import PriorityService from '@/services/priority.service';
import {AxiosResponse} from 'axios';
import {hasValue} from '@/shared/utils/has-value-util';

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
        // update state.priorityLibraries with a clone of the incoming list of priority libraries
        state.priorityLibraries = clone(priorityLibraries);
    },
    selectedPriorityLibraryMutator(state: any, priorityLibraryId: any) {
        if (any(propEq('id', priorityLibraryId), state.priorityLibraries)) {
            // find the existing priority library in state.priorityLibraries where the id matches priorityLibraryId,
            // clone it, then update state.selectedPriorityLibrary with the cloned, existing priority library
            state.selectedPriorityLibrary = clone(state.priorityLibraries
                .find((priorityLibrary: PriorityLibrary) =>
                    priorityLibrary.id === priorityLibraryId
                ) as PriorityLibrary
            );
        } else {
            // update state.selectedPriorityLibrary with a new empty priority library object
            state.selectedPriorityLibrary = clone(emptyPriorityLibrary);
        }
    },
    updatedSelectedPriorityLibraryMutator(state: any, updatedSelectedPriorityLibrary: PriorityLibrary) {
        // update the state.selectedPriorityLibrary with the updatedSelectedPriorityLibrary clone
        state.selectedPriorityLibrary = clone(updatedSelectedPriorityLibrary);
    },
    createdPriorityLibraryMutator(state: any, createdPriorityLibrary: PriorityLibrary) {
        // append the created priority library to a clone of state.priorityLibraries, then update state.priorityLibraries
        // with the clone
        state.priorityLibraries = append(createdPriorityLibrary, state.priorityLibraries);
    },
    updatedPriorityLibraryMutator(state: any, updatedPriorityLibrary: PriorityLibrary) {
        if (any(propEq('id', updatedPriorityLibrary.id), state.priorityLibraries)) {
            // clone state.priorityLibraries
            const priorityLibraries: PriorityLibrary[] = clone(state.priorityLibraries);
            // find the index of the existing priority library in the clone that has a matching id with the updated
            // priority library id
            const index: number = findIndex(propEq('id', updatedPriorityLibrary.id), priorityLibraries);
            // set the priority library at the specified index with the updated priority library
            priorityLibraries[index] = updatedPriorityLibrary;
            // update state.priorityLibraries with the clone
            state.priorityLibraries = priorityLibraries;
        }
    },
    scenarioPriorityLibraryMutator(state: any, scenarioPriorityLibrary: PriorityLibrary) {
        // update state.scenarioPriorityLibrary with a clone of the incoming scenario priority library
        state.scenarioPriorityLibrary = clone(scenarioPriorityLibrary);
    }
};

const actions = {
    selectPriorityLibrary({commit}: any, payload: any) {
        commit('selectedPriorityLibraryMutator', payload.priorityLibraryId);
    },
    updateSelectedPriorityLibrary({commit}: any, payload: any) {
        commit('updatedSelectedPriorityLibraryMutator', payload.updatedSelectedPriorityLibrary);
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
                    commit('selectedPriorityLibraryMutator', updatedPriorityLibrary.id);
                    dispatch('setSuccessMessage', {message: 'Successfully updated priority library'});
                }
            });
    },
    async getScenarioPriorityLibrary({commit}: any, payload: any) {
        if (payload.selectedScenarioId > 0) {
            await PriorityService.getScenarioPriorityLibrary(payload.selectedScenarioId)
                .then((response: AxiosResponse<PriorityLibrary>) => {
                    if (hasValue(response, 'data')) {
                        commit('scenarioPriorityLibraryMutator', response.data);
                        commit('updatedSelectedPriorityLibraryMutator', response.data);
                    }
                });
        } else {
            commit('scenarioPriorityLibraryMutator', emptyPriorityLibrary);
            commit('updatedSelectedPriorityLibraryMutator', emptyPriorityLibrary);
        }
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
            if (payload.operationType === 'update' || payload.operationType === 'replace') {
                const updatedPriorityLibrary: PriorityLibrary = convertFromMongoToVueModel(payload.fullDocument);
                commit('updatedPriorityLibraryMutator', updatedPriorityLibrary);
                if (state.selectedPriorityLibrary.id === updatedPriorityLibrary.id &&
                    !equals(state.selectedPriorityLibrary, updatedPriorityLibrary)) {
                    commit('selectedPriorityLibrary', updatedPriorityLibrary.id);
                    dispatch('setInfoMessage', {message: 'Library data has been changed from another source'});
                }
            }

            if (payload.operationType === 'insert') {
                const createdPriorityLibrary: PriorityLibrary = convertFromMongoToVueModel(payload.fullDocument);
                commit('createdPriorityLibraryMutator', createdPriorityLibrary);
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
