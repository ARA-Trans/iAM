import {Scenario} from '@/shared/models/iAM/scenario';
import {statusReference} from '@/firebase';
import ScenarioService from '@/services/scenario.service';
import moment from 'moment';
import {AxiosResponse} from 'axios';
import {http2XX, setStatusMessage} from '@/shared/utils/http-utils';

const state = {
    scenarios: [] as Scenario[],
};

const mutations = {
    scenariosMutator(state: any, scenarios: Scenario[]) {
        state.scenarios = [...scenarios];
    }
};

const actions = {
    getUserScenarios({dispatch, commit}: any, payload: any) {
        statusReference.on('value', (snapshot: any) => {
            const results = snapshot.val();
            let scenarios: Scenario[] = [];
            for (let key in results) {
                scenarios.push({
                    networkId: results[key].networkId,
                    scenarioId: results[key].simulationId,
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
            dispatch('setErrorMessage', {message: `Failed to get scenarios: ${error}`});
        });
    },
    async getScenarios({dispatch, commit}: any) {
        return await ScenarioService.getScenarios().then((response: AxiosResponse<any>) => {
            if (http2XX.test(response.status.toString())) {
                const data: any[] = response.data as any[];
                const scenarios: Scenario[] = data.map((scenarioData: any) => ({
                    networkId: scenarioData.networkId,
                    scenarioId: scenarioData.simulationId,
                    networkName: scenarioData.networkName,
                    simulationName: scenarioData.simulationName,
                    name: scenarioData.simulationName,
                    createdDate: scenarioData.created,
                    lastModifiedDate: scenarioData.lastRun,
                    status: 'success',
                    shared: false
                }));
                commit('scenariosMutator', scenarios);
            } else {
                dispatch('setErrorMessage', {message: `Detailed Report Download Failed${setStatusMessage(response)}`});
            }
        });
    },
    async createNewScenario({ commit }: any, payload: any) {
        return await ScenarioService.createScenario(payload.networkId, payload.scenarioName)
            .then((response: AxiosResponse<any>) => {

                const scenarioData = {
                    status: 'new',
                    owner: payload.userId,
                    sharedWith: [],
                    networkId: payload.networkId,
                    simulationId: response.data,
                    networkName: payload.networkName,
                    simulationName: payload.scenarioName,
                    created: moment().toISOString(),
                    lastModified: moment().toISOString()
                };
                statusReference.child('Scenario' + '_' + payload.networkId.toString() + '_' + response.data.toString()).set(scenarioData)
                    .then(() => {
                    })
                    .catch((error) => {
                        console.log(error);
                    });
                return response.status;
            })
            .catch((error: any) => { return error.response.status; });
    },
    runSimulation({ commit }: any, payload: any) {
        statusReference.once('value', (snapshot) => {
            if (snapshot.hasChild('Scenario' + '_' + payload.networkId.toString() + '_' + payload.simulationId.toString())) {
                const simulationData = {
                    status: 'Started',
                    networkId: payload.networkId,
                    simulationId: payload.simulationId,
                    networkName: payload.networkName,
                    simulationName: payload.simulationName,
                    lastModified: moment().toISOString(),
                    //[Note]: this will be removed
                    owner: payload.userId
                };
                statusReference.child('Scenario' + '_' + payload.networkId.toString() + '_' + payload.simulationId.toString()).update(simulationData)
                    .then(() => {
                        ScenarioService.runScenarioSimulation(payload.networkId, payload.networkName, payload.simulationId, payload.simulationName)
                            .then((simulation: any) => console.log(simulation))
                            .catch((error: any) => console.log(error));
                    })
                    .catch((error) => {
                        console.log(error);
                    });
            }
            else {
                const simulationData = {
                    status: 'Started',
                    owner: payload.userId,
                    sharedWith: [],
                    networkId: payload.networkId,
                    simulationId: payload.simulationId,
                    networkName: payload.networkName,
                    simulationName: payload.simulationName,
                    created: moment().toISOString(),
                    lastModified: moment().toISOString()
                };
                statusReference.child('Scenario' + '_' + payload.networkId.toString() + '_' + payload.simulationId.toString()).set(simulationData)
                    .then(() => {
                        ScenarioService.runScenarioSimulation(payload.networkId, payload.networkName, payload.simulationId, payload.simulationName)
                            .then((simulation: any) => console.log(simulation))
                            .catch((error: any) => console.log(error));
                    })
                    .catch((error) => {
                        console.log(error);
                    });
            }
        });
    }
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations
};
