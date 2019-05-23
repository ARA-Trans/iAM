import {emptyInvestmentLibrary, InvestmentLibrary} from '@/shared/models/iAM/investment';
import InvestmentEditorService from '@/services/investment-editor.service';
import {clone, any, propEq, append, findIndex} from 'ramda';
import {AxiosResponse} from 'axios';
import {http2XX, setStatusMessage} from '@/shared/utils/http-utils';

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
    selectedInvestmentLibraryMutator(state: any, investmentLibraryId: number) {
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
    scenarioInvestmentLibraryMutator(state: any, investmentForScenario: InvestmentLibrary[]) {
        // update state.investmentLibraries with a clone of the incoming list of investment libraries
        state.scenarioInvestmentLibrary = clone(investmentForScenario);
    }
};

const actions = {
    async getInvestmentLibraries({dispatch, commit}: any) {
        await new InvestmentEditorService().getInvestmentLibraries()
            .then((investmentLibraries: InvestmentLibrary[]) =>
                commit('investmentLibrariesMutator', investmentLibraries)
            )
            .catch((error: string) => dispatch('setErrorMessage', {message: error}));
    },
    selectInvestmentLibrary({commit}: any, payload: any) {
        commit('selectedInvestmentLibraryMutator', payload.investmentLibraryId);
    },
    updateSelectedInvestmentLibrary({commit}: any, payload: any) {
        commit('updatedSelectedInvestmentLibraryMutator', payload.updatedSelectedInvestmentLibrary);
    },
    async createInvestmentLibrary({dispatch, commit}: any, payload: any) {
        await InvestmentEditorService.createInvestmentLibrary(payload.createdInvestmentLibrary)
            .then((response: AxiosResponse<InvestmentLibrary>) => {
                if (http2XX.test(response.status.toString())) {
                    commit('createdInvestmentLibraryMutator', response.data);
                    commit('selectedInvestmentLibraryMutator', response.data.id);
                    dispatch('setSuccessMessage', {message: 'Successfully created investment library'});
                } else {
                    dispatch('setErrorMessage', {message: `Failed to create investment library${setStatusMessage(response)}`});
                }
            });
    },
    async updateInvestmentLibrary({dispatch, commit}: any, payload: any) {
        await new InvestmentEditorService().updateInvestmentLibrary(payload.updatedInvestmentLibrary)
            .then(() => {
                commit('updatedInvestmentLibraryMutator', payload.updatedInvestmentLibrary);
                commit('selectedInvestmentLibraryMutator', payload.updatedInvestmentLibrary.id);
            })
            .catch((error: string) => dispatch('setErrorMessage', {message: error}));
    },
    async getScenarioInvestmentLibrary({dispatch, commit}: any, payload: any) {
        if (payload.selectedScenarioId > 0) {
            await InvestmentEditorService.getScenarioInvestmentLibrary(payload.selectedScenarioId)
                .then((response: AxiosResponse<any>) => {
                    if (http2XX.test(response.status.toString())) {
                        const scenarioInvestmentLibrary: InvestmentLibrary = {
                            id: response.data.scenarioId,
                            name: response.data.name,
                            inflationRate: response.data.inflationRate,
                            discountRate: response.data.discountRate,
                            description: response.data.description,
                            budgetOrder: response.data.budgetNamesByOrder,
                            budgetYears: response.data.yearlyBudgets,
                        };
                        commit('scenarioInvestmentLibraryMutator', scenarioInvestmentLibrary);
                        commit('updatedSelectedInvestmentLibraryMutator', scenarioInvestmentLibrary);
                    } else {
                        dispatch('setErrorMessage', {message: `Failed to get scenario investment library${setStatusMessage(response)}`});
                    }
                });
        } else {
            commit('scenarioInvestmentLibraryMutator', emptyInvestmentLibrary);
            commit('updatedSelectedInvestmentLibraryMutator', emptyInvestmentLibrary);
        }
    },
    async saveScenarioInvestmentLibrary({dispatch, commit}: any, payload: any) {
        return await InvestmentEditorService.saveScenarioInvestmentLibrary(payload.updatedInvestmentScenario)
            .then((response: AxiosResponse<InvestmentLibrary>) => {
                if (http2XX.test(response.status.toString())) {
                    commit('scenarioInvestmentLibraryMutator', response.data);
                    commit('updatedSelectedInvestmentLibraryMutator', response.data);
                    dispatch('setSuccessMessage', {message: 'Successfully saved scenario investment library'});
                } else {
                    dispatch('setErrorMessage', {message: `Failed to save scenario investment library${setStatusMessage(response)}`});
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
