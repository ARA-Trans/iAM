import {emptyInvestmentLibrary, InvestmentLibrary} from '@/shared/models/iAM/investment';
import InvestmentEditorService from '@/services/investment-editor.service';
import {clone, any, propEq, append, findIndex, isNil} from 'ramda';
import {db} from '@/firebase';

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
        state.selectedInvestmentLibrary = updatedSelectedInvestmentLibrary;
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
        state.investmentForScenario = clone(investmentForScenario);
    }
};

const actions = {
    async getInvestmentLibraries({ commit }: any) {
        await new InvestmentEditorService().getInvestmentLibraries()
            .then((investmentLibraries: InvestmentLibrary[]) =>
                commit('investmentLibrariesMutator', investmentLibraries)
            );
    },
    selectInvestmentLibrary({commit}: any, payload: any) {
        commit('selectedInvestmentLibraryMutator', payload.investmentLibraryId);
    },
    updateSelectedInvestmentLibrary({commit}: any, payload: any) {
        commit('updatedSelectedInvestmentLibraryMutator', payload.updatedSelectedInvestmentLibrary);
    },
    async createInvestmentLibrary({ commit }: any, payload: any) {
        await new InvestmentEditorService().createInvestmentLibrary(payload.createdInvestmentLibrary)
            .then(() => {
                commit('createdInvestmentLibraryMutator', payload.createdInvestmentLibrary);
                commit('selectedInvestmentLibraryMutator', payload.createdInvestmentLibrary.id);
            });
    },
    async updateInvestmentLibrary({ commit }: any, payload: any) {
        await new InvestmentEditorService().updateInvestmentLibrary(payload.updatedInvestmentLibrary)
            .then(() => {
                commit('updatedInvestmentLibraryMutator', payload.updatedInvestmentLibrary);
                commit('selectedInvestmentLibraryMutator', payload.updatedInvestmentLibrary.id);
            });
    },
    async getScenarioInvestmentLibrary({ commit }: any, payload: any) {
        await new InvestmentEditorService().getScenarioInvestmentLibrary(payload.selectedScenarioId)
            .then((scenarioInvestmentLibrary: InvestmentLibrary) => {
                commit('scenarioInvestmentLibraryMutator', scenarioInvestmentLibrary);
                commit('updatedSelectedInvestmentLibraryMutator', scenarioInvestmentLibrary);
            });
    },
    async upsertScenarioInvestmentLibrary({ commit }: any, payload: any) {
        return await new InvestmentEditorService().upsertScenarioInvestmentLibrary(payload.updatedInvestmentScenario)
            .then((scenarioInvestmentLibrary: InvestmentLibrary) => {
                commit('scenarioInvestmentLibraryMutator', scenarioInvestmentLibrary);
                commit('updatedSelectedInvestmentLibraryMutator', scenarioInvestmentLibrary);
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
