import {Scenario} from '@/shared/models/iAM/scenario';
import {statusReference} from '@/firebase';
import ScenarioService from '@/services/scenario.service';
import moment from 'moment';
import append from 'ramda/es/append';

const state = {
    scenarios: [] as Scenario[],
};

const mutations = {
    scenariosMutator(state: any, scenarios: Scenario[]) {
        state.scenarios = [...scenarios];
    },
    appendScenarioMutator(state: any, scenario: Scenario) {
        state.scenarios = append(scenario, state.scenarios);
    }
};

const actions = {
    getUserScenarios({ commit }: any, payload: any) {
        statusReference.on('value', (snapshot: any) => {
            const results = snapshot.val();
            let scenarios: Scenario[] = [];
            for (let key in results) {
                scenarios.push({
                    networkId: results[key].networkId,
                    simulationId: results[key].simulationId,
                    networkName: results[key].networkName,
                    simulationName: results[key].simulationName,
                    name: key,
                    createdDate: results[key].created,
                    lastModifiedDate: results[key].lastModified,
                    status: results[key].status,
                    shared: false
                });
            }
            commit('scenariosMutator', scenarios);
        }, (error: any) => {
            console.log('error in fetching scenarios', error);
        });
    },

    async createNewScenario({ commit }: any, payload: any) {
        return await new ScenarioService().createNewScenario(payload.networkId, payload.scenarioName)
            .then((results: any) => {
                let scenario: Scenario = {
                    networkId: payload.networkId,
                    simulationId: results.data,
                    simulationName: payload.scenarioName,
                    networkName: payload.networkName,
                    createdDate: moment().toDate(),
                    lastModifiedDate: moment().toDate(),
                    shared: false,
                    status: 'Success',
                    name: payload.scenarioName
                }
                commit('appendScenarioMutator', scenario);
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
