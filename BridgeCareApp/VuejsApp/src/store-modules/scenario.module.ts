import {Scenario} from '../models/scenario';
import ScenarioService from '../services/scenario.service';

const state = {
    scenarios: [] as Scenario[]
};

const mutations = {
    // @ts-ignore
    scenariosMutator(state, scenarios) {
        state.scenarios = scenarios;
    }
};

const actions = {
    // @ts-ignore
    getUserScenarios({commit}, payload) {
        new ScenarioService().getUserScenarios(payload.userId)
            .then((scenarios: Scenario[]) => commit('scenariosMutator', scenarios))
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
