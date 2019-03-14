import {Scenario} from '@/shared/models/iAM/scenario';
import ScenarioService from '../services/scenario.service';
import * as R from 'ramda';

const state = {
    scenarios: [] as Scenario[]
};

const mutations = {
    scenariosMutator(state: any, scenarios: Scenario[]) {
        state.scenarios = R.clone(scenarios);
    }
};

const actions = {
    getUserScenarios({commit}: any, payload: any) {
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
