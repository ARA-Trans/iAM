import {InvestmentStrategy} from '@/shared/models/iAM/investment';
import InvestmentEditorService from '@/services/investment-editor.service';
import * as R from 'ramda';

const state = {
    investmentStrategies: [] as InvestmentStrategy[],
    budgets: [] as string[]
};

const mutations = {
    investmentStrategiesMutator(state: any, investmentStrategies: InvestmentStrategy[]) {
        state.investmentStrategies = R.clone(investmentStrategies);
    },
    savedInvestmentStrategyMutator(state: any, savedInvestmentStrategy: InvestmentStrategy) {
        if (R.any(R.propEq('simulationId', savedInvestmentStrategy.simulationId), state.investmentStrategies)) {
            state.investmentStrategies = state.investmentStrategies.map((existingInvestmentStrategy: InvestmentStrategy) => {
                if (existingInvestmentStrategy.simulationId === savedInvestmentStrategy.simulationId) {
                    return R.merge(existingInvestmentStrategy, savedInvestmentStrategy);
                }
                return existingInvestmentStrategy;
            });
        } else {
            state.investmentStrategies = R.append(savedInvestmentStrategy);
        }
    },
    budgetsMutator(state: any, budgets: string[]) {
        state.budgets = R.clone(budgets);
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
    async saveInvestmentStrategy({commit}: any, payload: any) {
        await new InvestmentEditorService().saveInvestmentStrategy(payload.savedInvestmentStrategy)
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
