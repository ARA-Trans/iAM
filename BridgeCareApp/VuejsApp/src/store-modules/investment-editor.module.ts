import {
    emptyInvestmentStrategy,
    InvestmentStrategy, InvestmentStrategyBudgetYear
} from '@/shared/models/iAM/investment';
import InvestmentEditorService from '@/services/investment-editor.service';
import { clone, any, propEq, append, findIndex, merge, isNil } from 'ramda';
import { db } from '@/firebase';

const state = {
    investmentStrategies: [] as InvestmentStrategy[],
    selectedInvestmentStrategy: {...emptyInvestmentStrategy} as InvestmentStrategy
};

const mutations = {
    investmentStrategiesMutator(state: any, investmentStrategies: InvestmentStrategy[]) {
        // update state.investmentStrategies with a clone of the incoming list of investment strategies
        state.investmentStrategies = clone(investmentStrategies);
    },
    selectedInvestmentStrategyMutator(state: any, investmentStrategyId: number) {
        if (any(propEq('id', investmentStrategyId), state.investmentStrategies)) {
            // find the existing investment strategy in state.investmentStrategies where the id matches investmentStrategyId,
            // clone it, then update state.selectedInvestmentStrategy with the cloned, existing investment strategy
            state.selectedInvestmentStrategy = clone(state.investmentStrategies
                .find((investmentStrategy: InvestmentStrategy) =>
                    investmentStrategy.id === investmentStrategyId
                ) as InvestmentStrategy);
        } else if (investmentStrategyId === null) {
            // update state.selectedInvestmentStrategy with a new empty investment strategy object
            state.selectedInvestmentStrategy = {...emptyInvestmentStrategy};
        }
    },
    updateSelectedInvestmentStrategyMutator(state: any, updatedSelectedInvestmentStrategy: InvestmentStrategy) {
        state.selectedInvestmentStrategy = updatedSelectedInvestmentStrategy;
    },
    createdInvestmentStrategyMutator(state: any, createdInvestmentStrategy: InvestmentStrategy) {
        // append the created investment strategy to a cloned list of state.investmentStrategies, then update
        // state.investmentStrategies with the cloned list
        state.investmentStrategies = append(createdInvestmentStrategy, state.investmentStrategies);
    },
    updatedInvestmentStrategyMutator(state: any, updatedInvestmentStrategy: InvestmentStrategy) {
        if (any(propEq('id', updatedInvestmentStrategy.id), state.investmentStrategies)) {
            // clone the list of investment strategies in state
            const investmentStrategies: InvestmentStrategy[] = clone(state.investmentStrategies);
            // find the index of the existing investment strategy in the cloned list of investment strategies that has
            // a matching id with the updated investment strategy
            const index: number = findIndex(propEq('id', updatedInvestmentStrategy.id), investmentStrategies);
            // set the investment strategies at the specified index with the updated investment strategy
            investmentStrategies[index] = updatedInvestmentStrategy;
            // update state.investmentStrategies with the cloned list of investment strategies
            state.investmentStrategies = investmentStrategies;
        }
    }
};

const actions = {
    async getInvestmentStrategies({ commit }: any) {
        db.ref('investmentLibraries').on('value', (snapshot: any) => {
            const results = snapshot.val();
            let investmentStrategies: InvestmentStrategy[] = [];
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
                investmentStrategies.push({
                    id: results[key].id,
                    name: results[key].name,
                    inflationRate: results[key].inflationRate,
                    discountRate: results[key].discountRate,
                    description: results[key].description,
                    budgetOrder: results[key].budgetOrder,
                    budgetYears: results[key].budgetYears,
                    deletedBudgetYearIds: results[key].deletedBudgetYearIds
                });
            }
            commit('investmentStrategiesMutator', investmentStrategies);
        }, (error: any) => {
            console.log('error in fetching investment libraries', error);
        });
        //await new InvestmentEditorService().getInvestmentStrategies()
        //    .then((investmentStrategies: InvestmentStrategy[]) =>
        //        commit('investmentStrategiesMutator', investmentStrategies)
        //    )
        //    .catch((error: any) => console.log(error));
    },
    selectInvestmentStrategy({commit}: any, payload: any) {
        commit('selectedInvestmentStrategyMutator', payload.investmentStrategyId);
    },

    updateSelectedInvestmentStrategy({commit}: any, payload: any) {
        commit('updateSelectedInvestmentStrategyMutator', payload.updatedInvestmentStrategy);
    },
    async createInvestmentStrategy({ commit }: any, payload: any) {
        db.ref('investmentLibraries').child('Investment_' + payload.createdInvestmentStrategy.id).set(payload.createdInvestmentStrategy)
            .then(() => {
                commit('createdInvestmentStrategyMutator', payload.createdInvestmentStrategy);
                commit('selectedInvestmentStrategyMutator', payload.createdInvestmentStrategy.id);
            })
            .catch((error: any) => console.log(error));
        //await new InvestmentEditorService().createInvestmentStrategy(payload.createdInvestmentStrategy)
        //    .then((createdInvestmentStrategy: InvestmentStrategy) => {
        //        commit('createdInvestmentStrategyMutator', createdInvestmentStrategy);
        //        commit('selectedInvestmentStrategyMutator', createdInvestmentStrategy.id);
        //    })
        //    .catch((error: any) => console.log(error));
    },
    async updateInvestmentStrategy({ commit }: any, payload: any) {
        db.ref('investmentLibraries').child('Investment_' + payload.updatedInvestmentStrategy.id).update(payload.updatedInvestmentStrategy)
            .then(() => {
                commit('updatedInvestmentStrategyMutator', payload.updatedInvestmentStrategy);
                commit('selectedInvestmentStrategyMutator', payload.updatedInvestmentStrategy.id);
            })
            .catch((error: any) => console.log(error));
        //await new InvestmentEditorService().updateInvestmentStrategy(payload.updatedInvestmentStrategy)
        //    .then((updatedInvestmentStrategy: InvestmentStrategy) => {
        //        commit('updatedInvestmentStrategyMutator', updatedInvestmentStrategy);
        //        commit('selectedInvestmentStrategyMutator', updatedInvestmentStrategy.id);
        //    })
        //    .catch((error: any) => console.log(error));
    }
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations
};
