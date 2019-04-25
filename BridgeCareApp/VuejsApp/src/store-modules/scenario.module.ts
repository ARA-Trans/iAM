import {Analysis, emptyAnalysis, emptyScenario, Scenario} from '@/shared/models/iAM/scenario';
import {any, propEq, findIndex} from 'ramda';
import {statusReference} from '@/firebase';
import ScenarioService from '@/services/scenario.service';

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
    },
    scenarioAnalysisMutator(state: any, analysis: Analysis) {
        if (state.selectedScenario.simulationId > 0) {
            const updatedScenario: Scenario = {...state.selectedScenario, analysis: analysis};
            const scenarios: Scenario[] = [...state.scenarios];
            const index = findIndex(
                (scenario: Scenario) => scenario.simulationId === updatedScenario.simulationId, scenarios
            );
            if (index !== -1) {
                scenarios[index] = updatedScenario;
                state.scenarios = scenarios;
            }
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
    },
    async applyAnalysisToScenario({commit}: any, payload: any) {
        await new ScenarioService().applyAnalysisToScenario(payload.scenarioAnalysisUpsertData)
            .then((analysis: Analysis) => {
                commit('scenarioAnalysisMutator', analysis);
                commit('selectedScenarioMutator', payload.scenarioAnalysisUpsertData.id);
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
