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
        } else if (investmentLibraryId === null) {
            // update state.selectedInvestmentLibrary with a new empty investment library object
            state.selectedInvestmentLibrary = {...emptyInvestmentLibrary};
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
        await db.ref('investmentLibraries').on('value', (snapshot: any) => {
            const results = snapshot.val();
            let investmentLibraries: InvestmentLibrary[] = [];
            for (let key in results) {
                if (isNil(results[key].budgetOrder)) {
                    results[key].budgetOrder = [];
                }
                if (isNil(results[key].budgetYears)) {
                    results[key].budgetYears = [];
                }
                if (isNil(results[key].deletedBudgetYearIds)) {
                    results[key].deletedBudgetYearIds = [];
                }
                investmentLibraries.push({
                    id: results[key].id,
                    name: results[key].name,
                    inflationRate: results[key].inflationRate,
                    discountRate: results[key].discountRate,
                    description: results[key].description,
                    budgetOrder: results[key].budgetOrder,
                    budgetYears: results[key].budgetYears
                });
            }
            commit('investmentLibrariesMutator', investmentLibraries);
        }, (error: any) => {
            console.log('error in fetching investment libraries', error);
        });
    },
    selectInvestmentLibrary({commit}: any, payload: any) {
        commit('selectedInvestmentLibraryMutator', payload.investmentLibraryId);
    },
    updateSelectedInvestmentLibrary({commit}: any, payload: any) {
        commit('updatedSelectedInvestmentLibraryMutator', payload.updatedInvestmentLibrary);
    },
    async createInvestmentLibrary({ commit }: any, payload: any) {
        await db.ref('investmentLibraries').child('Investment_' + payload.createdInvestmentLibrary.id).set(payload.createdInvestmentLibrary)
            .then(() => {
                commit('createdInvestmentLibraryMutator', payload.createdInvestmentLibrary);
                commit('selectedInvestmentLibraryMutator', payload.createdInvestmentLibrary.id);
            })
            .catch((error: any) => console.log(error));
    },
    async updateInvestmentLibrary({ commit }: any, payload: any) {
        await db.ref('investmentLibraries').child('Investment_' + payload.updatedInvestmentLibrary.id).update(payload.updatedInvestmentLibrary)
            .then(() => {
                commit('updatedInvestmentLibraryMutator', payload.updatedInvestmentLibrary);
                commit('selectedInvestmentLibraryMutator', payload.updatedInvestmentLibrary.id);
            })
            .catch((error: any) => console.log(error));
    },
    async getScenarioInvestmentLibrary({ commit }: any, payload: any) {
        await new InvestmentEditorService().getScenarioInvestmentLibrary(payload.selectedScenario)
            .then((investmentLibrary: any) => {
                const scenarioInvestmentLibrary: InvestmentLibrary = {
                    id: investmentLibrary[0].simulationId,
                    name: investmentLibrary[0].name,
                    inflationRate: investmentLibrary[0].inflationRate,
                    discountRate: investmentLibrary[0].discountRate,
                    description: investmentLibrary[0].description,
                    budgetOrder: investmentLibrary[0].budgetNamesByOrder,
                    budgetYears: investmentLibrary[0].yearlyBudgets
                };
                commit('scenarioInvestmentLibraryMutator', scenarioInvestmentLibrary);
            })
            .catch((error: any) => console.log(error));
    },
    async upsertScenarioInvestmentLibrary({ commit }: any, payload: any) {
        return await new InvestmentEditorService().updateScenarioInvestmentLibrary(payload.updatedInvestmentScenario)
            .then((results: any) => {
                return results.status;
            })
            .catch((error: any) => { return error.response.status; });
    }
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations
};
