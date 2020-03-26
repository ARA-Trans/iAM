import {CashFlowLibrary, emptyCashFlowLibrary} from '@/shared/models/iAM/cash-flow';
import {any, append, clone, equals, find, findIndex, propEq, reject, update} from 'ramda';
import CashFlowService from '@/services/cash-flow.service';
import {AxiosResponse} from 'axios';
import {hasValue} from '@/shared/utils/has-value-util';
import {convertFromMongoToVue} from '@/shared/utils/mongo-model-conversion-utils';
import {http2XX} from '@/shared/utils/http-utils';

const state = {
    cashFlowLibraries: [] as CashFlowLibrary[],
    selectedCashFlowLibrary: clone(emptyCashFlowLibrary) as CashFlowLibrary,
    scenarioCashFlowLibrary: clone(emptyCashFlowLibrary) as CashFlowLibrary,
};

const mutations = {
    cashFlowLibrariesMutator(state: any, cashFlowLibraries: CashFlowLibrary[]) {
        state.cashFlowLibraries = clone(cashFlowLibraries);
    },
    selectedCashFlowLibraryMutator(state: any, libraryId: string) {
        if (any(propEq('id', libraryId), state.cashFlowLibraries)) {
            state.selectedCashFlowLibrary = find(propEq('id', libraryId), state.cashFlowLibraries);
        } else if (state.scenarioCashFlowLibrary.id === libraryId) {
            state.selectedCashFlowLibrary = clone(state.scenarioCashFlowLibrary);
        } else {
            state.selectedCashFlowLibrary = clone(emptyCashFlowLibrary);
        }
    },
    createdCashFlowLibraryMutator(state: any, createdCashFlowLibrary: CashFlowLibrary) {
        state.cashFlowLibraries = append(createdCashFlowLibrary, state.cashFlowLibraries);
    },
    updatedCashFlowLibraryMutator(state: any, updatedCashFlowLibrary: CashFlowLibrary) {
        state.cashFlowLibraries = update(
            findIndex(propEq('id', updatedCashFlowLibrary.id), state.cashFlowLibraries),
            updatedCashFlowLibrary,
            state.cashFlowLibraries
        );
    },
    deletedCashFlowLibraryMutator(state: any, deletedCashFlowLibraryId: string) {
        if (any(propEq('id', deletedCashFlowLibraryId), state.cashFlowLibraries)) {
            state.cashFlowLibraries = reject(
                (library: CashFlowLibrary) => deletedCashFlowLibraryId === library.id,
                state.cashFlowLibraries
            );
        }
    },
    scenarioCashFlowLibraryMutator(state: any, scenarioCashFlowLibrary: CashFlowLibrary) {
        state.scenarioCashFlowLibrary = clone(scenarioCashFlowLibrary);
    }
};

const actions = {
    selectCashFlowLibrary({commit}: any, payload: any) {
        commit('selectedCashFlowLibraryMutator', payload.selectedLibraryId);
    },
    async getCashFlowLibraries({commit}: any) {
        await CashFlowService.getCashFlowLibraries().then((response: AxiosResponse<any[]>) => {
            if (hasValue(response, 'data')) {
                const cashFlowLibraries: CashFlowLibrary[] = response.data
                    .map((data: any) => convertFromMongoToVue(data));
                commit('cashFlowLibrariesMutator', cashFlowLibraries);
            }
        });
    },
    async createCashFlowLibrary({dispatch, commit}: any, payload: any) {
        await CashFlowService.createCashFlowLibrary(payload.createdCashFlowLibrary)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    const createdCashFlowLibrary: CashFlowLibrary = convertFromMongoToVue(response.data);
                    commit('createdCashFlowLibraryMutator', createdCashFlowLibrary);
                    commit('selectedCashFlowLibraryMutator', createdCashFlowLibrary.id);
                    dispatch('setSuccessMessage', {message: 'Successfully created cash flow library'});
                }
            });
    },
    async updateCashFlowLibrary({dispatch, commit}: any, payload: any) {
        await CashFlowService.updateCashFlowLibrary(payload.updatedCashFlowLibrary)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    const updatedCashFlowLibrary: CashFlowLibrary = convertFromMongoToVue(response.data);
                    commit('updatedCashFlowLibraryMutator', updatedCashFlowLibrary);
                    commit('selectedCashFlowLibraryMutator', updatedCashFlowLibrary.id);
                    dispatch('setSuccessMessage', {message: 'Successfully updated cash flow library'});
                }
            });
    },
    async deleteCashFlowLibrary({dispatch, commit, state}: any, payload: any) {
        await CashFlowService.deleteCashFlowLibrary(payload.cashFlowLibrary)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                    commit('deletedCashFlowLibraryMutator', payload.cashFlowLibrary.id);
                    dispatch('setSuccessMessage', {message: 'Successfully deleted cash flow library'});
                }
            });
    },
    async getScenarioCashFlowLibrary({commit}: any, payload: any) {
        await CashFlowService.getScenarioCashFlowLibrary(payload.selectedScenarioId)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    commit('scenarioCashFlowLibraryMutator', response.data);
                    commit('selectedCashFlowLibraryMutator', response.data.id);
                }
            });
    },
    async saveScenarioCashFlowLibrary({dispatch, commit}: any, payload: any) {
        await CashFlowService.saveScenarioCashFlowLibrary(payload.scenarioCashFlowLibrary, payload.objectIdMOngoDBForScenario)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    commit('scenarioCashFlowLibraryMutator', response.data);
                    dispatch('setSuccessMessage', {message: 'Successfully saved scenario cash flow library'});
                }
            });
    },
    async socket_cashFlowLibrary({dispatch, state, commit}: any, payload: any) {
        if (hasValue(payload, 'operationType')) {
            if (hasValue(payload, 'fullDocument')) {
                const cashFlowLibrary: CashFlowLibrary = convertFromMongoToVue(payload.fullDocument);
                switch (payload.operationType) {
                    case 'update':
                    case 'replace':
                        commit('updatedCashFlowLibraryMutator', cashFlowLibrary);
                        if (state.selectedCashFlowLibrary.id === cashFlowLibrary.id && !equals(state.selectedCashFlowLibrary, cashFlowLibrary)) {
                            commit('selectedCashFlowLibraryMutator', cashFlowLibrary.id);
                            dispatch('setInfoMessage', {message: `Cash flow library '${cashFlowLibrary.name}' has been changed from another source`});
                        }
                        break;
                    case 'insert':
                        if (!any(propEq('id', cashFlowLibrary.id), state.cashFlowLibraries)) {
                            commit('createdCashFlowLibraryMutator', cashFlowLibrary);
                            dispatch('setInfoMessage', {message: `Cash flow library '${cashFlowLibrary.name}' has been created from another source`});
                        }
                }
            } else if (hasValue(payload, 'documentKey')) {
                if (any(propEq('id', payload.documentKey._id), state.cashFlowLibraries)) {
                    const deletedCashFlowLibrary: CashFlowLibrary = find(propEq('id', payload.documentKey._id), state.cashFlowLibraries);
                    commit('deletedCashFlowLibraryMutator', payload.documentKey._id);

                    if (deletedCashFlowLibrary.id === state.selectedCashFlowLibrary) {
                        if (!equals(state.scenarioCashFlowLibrary, emptyCashFlowLibrary)) {
                            commit('selectedCashFlowLibraryMutator', state.scenarioCashFlowLibrary.id);
                        } else {
                            commit('selectedCashFlowLibraryMutator', null);
                        }
                    }

                    dispatch('setInfoMessage', {message: `Cash flow library ${deletedCashFlowLibrary.name} has been deleted from another source`});
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
