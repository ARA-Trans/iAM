import {InvestmentStrategy} from '@/shared/models/iAM/investment';
import InvestmentEditorService from '@/services/investment-editor.service';
import {clone, any, propEq, merge, append} from 'ramda';

const state = {
    investmentStrategies: [] as InvestmentStrategy[],
    budgets: [] as string[]
};

const mutations = {
    investmentStrategiesMutator(state: any, investmentStrategies: InvestmentStrategy[]) {
        state.investmentStrategies = clone(investmentStrategies);
    },
    savedInvestmentStrategyMutator(state: any, savedInvestmentStrategy: InvestmentStrategy) {
        if (any(propEq('simulationId', savedInvestmentStrategy.simulationId), state.investmentStrategies)) {
            state.investmentStrategies = state.investmentStrategies.map((existingInvestmentStrategy: InvestmentStrategy) => {
                if (existingInvestmentStrategy.simulationId === savedInvestmentStrategy.simulationId) {
                    return merge(existingInvestmentStrategy, savedInvestmentStrategy);
                }
                return existingInvestmentStrategy;
            });
        } else {
            state.investmentStrategies = append(savedInvestmentStrategy, state.investmentStrategies);
        }
    },
    budgetsMutator(state: any, budgets: string[]) {
        state.budgets = clone(budgets);
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
    async saveInvestmentStrategyToLibrary({commit}: any, payload: any) {
        await new InvestmentEditorService().saveInvestmentStrategyToLibrary(payload.savedInvestmentStrategy)
            .then((investmentStrategy: InvestmentStrategy) =>
                commit('savedInvestmentStrategyMutator', investmentStrategy)
            )
            .catch((error: any) => console.log(error));
    },
    setBudgets({commit}: any, payload: any) {
        commit('budgetsMutator', payload.budgets);
    }
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations
};
