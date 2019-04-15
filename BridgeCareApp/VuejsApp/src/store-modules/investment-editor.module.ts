import {
    emptyInvestmentStrategy,
    InvestmentStrategy, InvestmentStrategyBudgetYear
} from '@/shared/models/iAM/investment';
import InvestmentEditorService from '@/services/investment-editor.service';
import {clone, any, propEq, append, findIndex, merge} from 'ramda';

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
    async getInvestmentStrategies({commit}: any) {
        await new InvestmentEditorService().getInvestmentStrategies()
            .then((investmentStrategies: InvestmentStrategy[]) =>
                commit('investmentStrategiesMutator', investmentStrategies)
            )
            .catch((error: any) => console.log(error));
    },
    selectInvestmentStrategy({commit}: any, payload: any) {
        commit('selectedInvestmentStrategyMutator', payload.investmentStrategyId);
    },

    updateSelectedInvestmentStrategy({commit}: any, payload: any) {
        commit('updateSelectedInvestmentStrategyMutator', payload.updatedInvestmentStrategy);
    },
    async createInvestmentStrategy({commit}: any, payload: any) {
        await new InvestmentEditorService().createInvestmentStrategy(payload.createdInvestmentStrategy)
            .then((createdInvestmentStrategy: InvestmentStrategy) => {
                commit('createdInvestmentStrategyMutator', createdInvestmentStrategy);
                commit('selectedInvestmentStrategyMutator', createdInvestmentStrategy.id);
            })
            .catch((error: any) => console.log(error));
    },
    async updateInvestmentStrategy({commit}: any, payload: any) {
        await new InvestmentEditorService().updateInvestmentStrategy(payload.updatedInvestmentStrategy)
            .then((updatedInvestmentStrategy: InvestmentStrategy) => {
                commit('updatedInvestmentStrategyMutator', updatedInvestmentStrategy);
                commit('selectedInvestmentStrategyMutator', updatedInvestmentStrategy.id);
            })
            .catch((error: any) => console.log(error));
    }
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations
};
