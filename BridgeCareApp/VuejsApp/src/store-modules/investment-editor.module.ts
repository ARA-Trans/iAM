import {emptyInvestmentLibrary, InvestmentLibrary} from '@/shared/models/iAM/investment';
import InvestmentEditorService from '@/services/investment-editor.service';
import {clone, any, propEq, append, findIndex} from 'ramda';

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
        await new InvestmentEditorService().createInvestmentLibrary(payload.createdInvestmentLibrary)
            .then(() => {
                commit('createdInvestmentLibraryMutator', payload.createdInvestmentLibrary);
                commit('selectedInvestmentLibraryMutator', payload.createdInvestmentLibrary.id);
            })
            .catch((error: string) => dispatch('setErrorMessage', {message: error}));
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
            await new InvestmentEditorService().getScenarioInvestmentLibrary(payload.selectedScenarioId)
                .then((data: any) => {
                    const scenarioInvestmentLibrary: InvestmentLibrary = {
                        id: data.scenarioId,
                        name: data.name,
                        inflationRate: data.inflationRate,
                        discountRate: data.discountRate,
                        description: data.description,
                        budgetOrder: data.budgetNamesByOrder,
                        budgetYears: data.yearlyBudgets,
                    };
                    commit('scenarioInvestmentLibraryMutator', scenarioInvestmentLibrary);
                    commit('updatedSelectedInvestmentLibraryMutator', scenarioInvestmentLibrary);
                })
                // TODO: uncomment when service has been updated to return an InvestmentLibrary
                /*.then((scenarioInvestmentLibrary: InvestmentLibrary) => {
                    commit('scenarioInvestmentLibraryMutator', scenarioInvestmentLibrary);
                    commit('updatedSelectedInvestmentLibraryMutator', scenarioInvestmentLibrary);
                });*/
                .catch((error: string) => dispatch('setErrorMessage', {message: error}));
        } else {
            commit('scenarioInvestmentLibraryMutator', emptyInvestmentLibrary);
            commit('updatedSelectedInvestmentLibraryMutator', emptyInvestmentLibrary);
        }
    },
    async upsertScenarioInvestmentLibrary({dispatch, commit}: any, payload: any) {
        return await new InvestmentEditorService().upsertScenarioInvestmentLibrary(payload.updatedInvestmentScenario)
            .then((scenarioInvestmentLibrary: InvestmentLibrary) => {
                commit('scenarioInvestmentLibraryMutator', scenarioInvestmentLibrary);
                commit('updatedSelectedInvestmentLibraryMutator', scenarioInvestmentLibrary);
            })
            .catch((error: string) => dispatch('setErrorMessage', {message: error}));
    }
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations
};
