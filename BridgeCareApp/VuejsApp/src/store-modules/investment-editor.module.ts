import {emptyInvestmentStrategy, InvestmentStrategy} from '@/models/iAM/investment';
import InvestmentEditorService from '@/services/investment-editor.service';

const state = {
    investmentStrategies: [] as InvestmentStrategy[],
    investmentStrategyDetail: emptyInvestmentStrategy as InvestmentStrategy
};

const mutations = {
    investmentStrategiesMutator(state: any, investmentStrategies: InvestmentStrategy[]) {
        state.investmentStrategies = investmentStrategies;
    }
};

const actions = {
    getInvestmentStrategies({commit}: any, payload: any) {
        new InvestmentEditorService().getInvestmentStrategies(payload.network)
            .then((investmentStrategies: InvestmentStrategy[]) =>
                commit('investmentStrategiesMutator', investmentStrategies)
            )
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
