import {emptyInvestmentLibrary, InvestmentLibrary, InvestmentLibraryBudgetYear} from '@/shared/models/iAM/investment';
import InvestmentEditorService from '@/services/investment-editor.service';
import {clone, any, propEq, append, findIndex, equals, reject} from 'ramda';
import {AxiosResponse} from 'axios';
import {hasValue} from '@/shared/utils/has-value-util';
import { http2XX } from '@/shared/utils/http-utils';

const convertFromMongoToVueModel = (data: any) => {
    const investmentLibrary: any = {
        ...data,
        id: data._id,
        budgetYears: data.budgetYears
            .map((budgetYear: any) => {
                const subData: any = {
                    ...budgetYear,
                    id: budgetYear._id
                };
                delete subData._id;
                delete subData.__v;
                return subData as InvestmentLibraryBudgetYear;
            })
    };
    delete investmentLibrary._id;
    delete investmentLibrary.__v;
    return investmentLibrary as InvestmentLibrary;
};

const state = {
    investmentLibraries: [] as InvestmentLibrary[],
    scenarioInvestmentLibrary: clone(emptyInvestmentLibrary) as InvestmentLibrary,
    selectedInvestmentLibrary: clone(emptyInvestmentLibrary) as InvestmentLibrary
};

const mutations = {
    investmentLibrariesMutator(state: any, investmentLibraries: InvestmentLibrary[]) {
        // update state.investmentLibraries with a clone of the incoming list of investment libraries
        state.investmentLibraries = clone(investmentLibraries);
    },
    selectedInvestmentLibraryMutator(state: any, investmentLibraryId: any) {
        if (any(propEq('id', investmentLibraryId), state.investmentLibraries)) {
            // find the existing investment library in state.investmentLibraries where the id matches investmentLibraryId,
            // clone it, then update state.selectedInvestmentLibrary with the cloned, existing investment library
            state.selectedInvestmentLibrary = clone(state.investmentLibraries
                .find((investmentLibrary: InvestmentLibrary) =>
                    investmentLibrary.id === investmentLibraryId
                ) as InvestmentLibrary);
        } else {
            // update state.selectedInvestmentLibrary with a new empty investment library object
            state.selectedInvestmentLibrary = clone(emptyInvestmentLibrary);
        }
    },
    updatedSelectedInvestmentLibraryMutator(state: any, updatedSelectedInvestmentLibrary: InvestmentLibrary) {
        state.selectedInvestmentLibrary = clone(updatedSelectedInvestmentLibrary);
    },
    createdInvestmentLibraryMutator(state: any, createdInvestmentLibrary: InvestmentLibrary) {
        // append the created investment library to a cloned list of state.investmentLibraries, then update
        // state.investmentLibraries with the cloned list
        state.investmentLibraries = append(createdInvestmentLibrary, state.investmentLibraries);
    },
    updatedInvestmentLibraryMutator(state: any, updatedInvestmentLibrary: InvestmentLibrary) {
        if (any(propEq('id', updatedInvestmentLibrary.id), state.investmentLibraries)) {
            // clone the list of investment libraries in state
            const investmentLibraries: InvestmentLibrary[] = clone(state.investmentLibraries);
            // find the index of the existing investment library in the cloned list of investment libraries that has
            // a matching id with the updated investment library
            const index: number = findIndex(propEq('id', updatedInvestmentLibrary.id), investmentLibraries);
            // set the investment libraries at the specified index with the updated investment library
            investmentLibraries[index] = updatedInvestmentLibrary;
            // update state.investmentLibraries with the cloned list of investment libraries
            state.investmentLibraries = investmentLibraries;
        }
    },
    scenarioInvestmentLibraryMutator(state: any, scenarioInvestmentLibrary: InvestmentLibrary) {
        // update state.investmentLibraries with a clone of the incoming list of investment libraries
        state.scenarioInvestmentLibrary = clone(scenarioInvestmentLibrary);
    },
    deletedInvestmentLibraryMutator(state: any, deletedInvestmentLibraryId: string) {
        if (any(propEq('id', deletedInvestmentLibraryId), state.investmentLibraries)) {
            state.investmentLibraries = reject(
                (library: InvestmentLibrary) => deletedInvestmentLibraryId === library.id,
                state.investmentLibraries
            );
        }
    }
};

const actions = {
    selectInvestmentLibrary({commit}: any, payload: any) {
        commit('selectedInvestmentLibraryMutator', payload.investmentLibraryId);
    },
    updateSelectedInvestmentLibrary({commit}: any, payload: any) {
        commit('updatedSelectedInvestmentLibraryMutator', payload.updatedSelectedInvestmentLibrary);
    },
    async getInvestmentLibraries({commit}: any) {
        await InvestmentEditorService.getInvestmentLibraries()
            .then((response: AxiosResponse<any[]>) => {
                if (hasValue(response, 'data')) {
                    const investmentLibraries: InvestmentLibrary[] = response.data
                        .map((data: any) => {
                            return convertFromMongoToVueModel(data);
                        });
                    commit('investmentLibrariesMutator', investmentLibraries);
                }
            });
    },
    async createInvestmentLibrary({dispatch, commit}: any, payload: any) {
        await InvestmentEditorService.createInvestmentLibrary(payload.createdInvestmentLibrary)
            .then((response: AxiosResponse<any>) => {
                if (hasValue(response, 'data')) {
                    const createdInvestmentLibrary: InvestmentLibrary = convertFromMongoToVueModel(response.data);
                    commit('createdInvestmentLibraryMutator', createdInvestmentLibrary);
                    dispatch('setSuccessMessage', {message: 'Successfully created investment library'});
                }
            });
    },
    async updateInvestmentLibrary({dispatch, commit}: any, payload: any) {
        await InvestmentEditorService.updateInvestmentLibrary(payload.updatedInvestmentLibrary)
            .then((response: AxiosResponse<any>) => {
                if (hasValue(response, 'data')) {
                    const updatedInvestmentLibrary: InvestmentLibrary = convertFromMongoToVueModel(response.data);
                    commit('updatedInvestmentLibraryMutator', updatedInvestmentLibrary);
                    commit('selectedInvestmentLibraryMutator', updatedInvestmentLibrary.id);
                    dispatch('setSuccessMessage', {message: 'Successfully updated investment library'});
                }
            });
    },
    async deleteInvestmentLibrary({dispatch, commit}: any, payload: any) {
        await InvestmentEditorService.deleteInvestmentLibrary(payload.investmentLibrary)
            .then((response: AxiosResponse<any>) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                commit('deletedInvestmentLibraryMutator', payload.investmentLibrary.id);
                dispatch('setSuccessMessage', {message: 'Successfully deleted investment library'});
                }
            });
    },
    async getScenarioInvestmentLibrary({dispatch, commit}: any, payload: any) {
        if (payload.selectedScenarioId !== '0') {
            await InvestmentEditorService.getScenarioInvestmentLibrary(payload.selectedScenarioId)
                .then((response: AxiosResponse<InvestmentLibrary>) => {
                    if (hasValue(response, 'data')) {
                        commit('scenarioInvestmentLibraryMutator', response.data);
                        commit('updatedSelectedInvestmentLibraryMutator', response.data);
                    }
                });
        } else {
            commit('scenarioInvestmentLibraryMutator', emptyInvestmentLibrary);
            commit('updatedSelectedInvestmentLibraryMutator', emptyInvestmentLibrary);
        }
    },
    async saveScenarioInvestmentLibrary({dispatch, commit}: any, payload: any) {
        await InvestmentEditorService.saveScenarioInvestmentLibrary(payload.saveScenarioInvestmentLibraryData)
            .then((response: AxiosResponse<InvestmentLibrary>) => {
                if (hasValue(response, 'data')) {
                    commit('scenarioInvestmentLibraryMutator', response.data);
                    dispatch('setSuccessMessage', {message: 'Successfully saved scenario investment library'});
                }
            });
    },
    async socket_investmentLibrary({dispatch, state, commit}: any, payload: any) {
        if (hasValue(payload, 'operationType') && hasValue(payload, 'fullDocument')) {
            if (payload.operationType == 'update' || payload.operationType == 'replace') {
                const updatedInvestmentLibrary: InvestmentLibrary = convertFromMongoToVueModel(payload.fullDocument);
                commit('updatedInvestmentLibraryMutator', updatedInvestmentLibrary);
                if (state.selectedInvestmentLibrary.id === updatedInvestmentLibrary.id &&
                    !equals(state.selectedInvestmentLibrary, updatedInvestmentLibrary)) {
                    commit('selectedInvestmentLibraryMutator', updatedInvestmentLibrary.id);
                    dispatch('setInfoMessage', {message: 'Library data has been changed from another source'});
                }
            }

            if (payload.operationType == 'insert') {
                const createdInvestmentLibrary: InvestmentLibrary = convertFromMongoToVueModel(payload.fullDocument);
                commit('createdInvestmentLibraryMutator', createdInvestmentLibrary);
            }
        } else if (hasValue(payload, 'operationType') && payload.operationType === 'delete') {
            if (any(propEq('id', payload.documentKey._id), state.investmentLibraries)) {
                commit('deletedInvestmentLibraryMutator', payload.documentKey._id);
                dispatch('setInfoMessage',
                    {message: `An investment library has been deleted from another source`}
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
