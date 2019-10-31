import {CashFlowLibrary, emptyCashFlowLibrary} from '@/shared/models/iAM/cash-flow';
import {clone, append, update, findIndex, propEq, equals, any} from 'ramda';
import CashFlowService from '@/services/cash-flow.service';
import {AxiosResponse} from 'axios';
import {hasValue} from '@/shared/utils/has-value-util';

const convertFromMongoToVueModel = (data: any) => {
    const cashFlowLibrary: any = {
        ...data,
        id: data._id,
        parameters: data.parameters.map((parameter: any) => {
            const subData: any = {
                ...parameter,
                id: parameter._id
            };
            delete subData._id;
            delete subData.__v;
            return subData;
        }),
        durations: data.durations.map((duration: any) => {
            const subData: any = {
                ...duration,
                id: duration._id
            };
            delete subData._id;
            delete subData.__v;
            return subData;
        })
    };
    delete data._id;
    delete data.__v;
    return cashFlowLibrary as CashFlowLibrary;
};

const state = {
    cashFlowLibraries: []  as CashFlowLibrary[],
    selectedCashFlowLibrary: clone(emptyCashFlowLibrary) as CashFlowLibrary,
    scenarioCashFlowLibrary: clone(emptyCashFlowLibrary) as CashFlowLibrary
};

const mutations = {
    cashFlowLibrariesMutator(state: any, cashFlowLibraries: CashFlowLibrary[]) {
        state.cashFlowLibraries = clone(cashFlowLibraries);
    },
    selectedCashFlowLibraryMutator(state: any, selectedCashFlowLibrary: CashFlowLibrary) {
        state.selectedCashFlowLibrary = clone(selectedCashFlowLibrary);
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
    scenarioCashFlowLibraryMutator(state: any, scenarioCashFlowLibrary: CashFlowLibrary) {
        state.scenarioCashFlowLibrary = clone(scenarioCashFlowLibrary);
    }
};

const actions = {
    selectCashFlowLibrary({commit}: any, payload: any) {
        commit('selectedCashFlowLibraryMutator', payload.selectedCashFlowLibrary);
    },
    async getCashFlowLibraries({commit}: any) {
        await CashFlowService.getCashFlowLibraries().then((response: AxiosResponse<any[]>) => {
            if (hasValue(response, 'data')) {
                const cashFlowLibraries: CashFlowLibrary[] = response.data
                    .map((data: any) => convertFromMongoToVueModel(data));
                commit('cashFlowLibrariesMutator', cashFlowLibraries);
            }
        });
    },
    async createCashFlowLibrary({dispatch, commit}: any, payload: any) {
        await CashFlowService.createCashFlowLibrary(payload.createdCashFlowLibrary)
            .then((response: AxiosResponse<any>) => {
                if (hasValue(response, 'data')) {
                    const createdCashFlowLibrary: CashFlowLibrary = convertFromMongoToVueModel(response.data);
                    commit('createdCashFlowLibraryMutator', createdCashFlowLibrary);
                    commit('selectedCashFlowLibraryMutator', createdCashFlowLibrary);
                    dispatch('setSuccessMessage', {message: 'Successfully created cash flow library'});
                }
            });
    },
    async updateCashFlowLibrary({dispatch, commit}: any, payload: any) {
        await CashFlowService.updateCashFlowLibrary(payload.updatedCashFlowLibrary)
            .then((response: AxiosResponse<any>) => {
                if (hasValue(response, 'data')) {
                    const updatedCashFlowLibrary: CashFlowLibrary = convertFromMongoToVueModel(response.data);
                    commit('updatedCashFlowLibraryMutator', updatedCashFlowLibrary);
                    commit('selectedCashFlowLibraryMutator', updatedCashFlowLibrary);
                    dispatch('setSuccessMessage', {message: 'Successfully updated cash flow library'});
                }
            });
    },
    async getScenarioCashFlowLibrary({commit}: any, payload: any) {
        await CashFlowService.getScenarioCashFlowLibrary(payload.selectedScenarioId)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    commit('scenarioCashFlowLibraryMutator', response.data);
                    commit('selectedCashFlowLibraryMutator', response.data);
                }
            });
    },
    async saveScenarioCashFlowLibrary({dispatch, commit}: any, payload: any) {
        await CashFlowService.saveScenarioCashFlowLibrary(payload.scenarioCashFlowLibrary)
            .then((response: AxiosResponse<any>) => {
                if (hasValue(response, 'data')) {
                    commit('scenarioCashFlowLibraryMutator', response.data);
                    commit('selectedCashFlowLibraryMutator', response.data);
                    dispatch('setSuccessMessage', {message: 'Successfully saved scenario cash flow library'});
                }
            });
    },
    async socket_cashFlowLibrary({dispatch, state, commit}: any, payload: any) {
        if (hasValue(payload, 'operationType') && hasValue(payload, 'fullDocument')) {
            const cashFlowLibrary: CashFlowLibrary = convertFromMongoToVueModel(payload.fullDocument);
            switch (payload.operationType) {
                case 'update':
                case 'replace':
                    commit('updatedCashFlowLibraryMutator', cashFlowLibrary);
                    if (state.selectedCashFlowLibrary.id === cashFlowLibrary.id &&
                        !equals(state.selectedCashFlowLibrary, cashFlowLibrary)) {
                        commit('selectedCashFlowLibraryMutator', cashFlowLibrary);
                        dispatch('setInfoMessage',
                            {message: `Cash flow library '${cashFlowLibrary.name}' has been changed from another source`}
                        );
                    }
                    break;
                case 'insert':
                    if (!any(propEq('id', cashFlowLibrary.id), state.cashFlowLibraries)) {
                        commit('createdCashFlowLibraryMutator', cashFlowLibrary);
                        dispatch('setInfoMessage',
                            {message: `Cash flow library '${cashFlowLibrary.name}' has been created from another source`}
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
