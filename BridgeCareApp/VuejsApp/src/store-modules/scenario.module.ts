import {Scenario} from '@/shared/models/iAM/scenario';
import {statusReference} from '@/firebase';
import ScenarioService from '@/services/scenario.service';
import moment from 'moment';
import {clone} from 'ramda';

const state = {
    scenarios: [] as Scenario[],
    benefitAttributes: [] as string[]
};

const mutations = {
    scenariosMutator(state: any, scenarios: Scenario[]) {
        state.scenarios = clone(scenarios);
    },
    benefitAttributesMutator(state: any, benefitAttributes: string[]) {
        state.benefitAttributes = clone(benefitAttributes);
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

    // Gets the scenarios/simulation from the SQL database
    async getScenarios({ commit }: any) {
        return await new ScenarioService().getScenarios()
            .then((results: any) => {
                let scenarios: Scenario[] = [];
                for (let key in results) {
                    scenarios.push({
                        networkId: results[key].networkId,
                        simulationId: results[key].simulationId,
                        networkName: results[key].networkName,
                        simulationName: results[key].simulationName,
                        name: results[key].simulationName,
                        createdDate: results[key].created,
                        lastModifiedDate: results[key].lastRun,
                        status: 'success',
                        shared: false
                    });
                }
                commit('scenariosMutator', scenarios);
            });
    },

    async createNewScenario({ commit }: any, payload: any) {
        return await new ScenarioService().createNewScenario(payload.networkId, payload.scenarioName)
            .then((results: any) => {

                const scenarioData = {
                    status: 'New scenario',
                    owner: payload.userId,
                    sharedWith: [],
                    networkId: payload.networkId,
                    simulationId: results.data,
                    networkName: payload.networkName,
                    simulationName: payload.scenarioName,
                    created: moment().toISOString(),
                    lastModified: moment().toISOString()
                };
                statusReference.child('Scenario' + '_' + payload.networkId.toString() + '_' + results.data.toString()).set(scenarioData)
                    .then(() => {
                    })
                    .catch((error) => {
                        console.log(error);
                    });
                return results.status;
            })
            .catch((error: any) => { return error.response.status; });
    },
    async getBenefitAttributes({dispatch, commit}: any) {
        await new ScenarioService().getBenefitAttributes()
            .then((benefitAttributes: string[]) => commit('benefitAttributesMutator', benefitAttributes))
            .catch((error: string) => dispatch('setErrorMessage', {message: error}));
    }
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations
};
