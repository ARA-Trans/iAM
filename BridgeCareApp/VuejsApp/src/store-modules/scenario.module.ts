import {Analysis, emptyAnalysis, emptyScenario, Scenario} from '@/shared/models/iAM/scenario';
import {any, propEq} from 'ramda';
import {statusReference} from '@/firebase';

const state = {
    scenarios: [] as Scenario[],
    selectedScenario: {...emptyScenario} as Scenario
};

const mutations = {
    scenariosMutator(state: any, scenarios: Scenario[]) {
            state.scenarios = [...scenarios];
    },
    selectedScenarioMutator(state: any, simulationId: number) {
        if (any(propEq('simulationId', simulationId), state.scenarios)) {
            state.selectedScenario = {...state.scenarios
                .find((scenario: Scenario) => scenario.simulationId === simulationId) as Scenario
            };
        } else {
            state.selectedScenario = {...emptyScenario};
        }
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
                    shared: false,
                    analysis: {...emptyAnalysis} as Analysis
                });
            }
            // TODO: remove mock scenario data when actual data can be provided

            // TODO: end
            commit('scenariosMutator', scenarios);
        }, (error: any) => {
            console.log('error in fetching scenarios', error);
        });
    },
    setSelectedScenario({commit}: any, payload: any) {
        commit('selectedScenarioMutator', payload.simulationId);
    }
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations
};
