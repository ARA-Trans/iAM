import {Scenario} from '@/shared/models/iAM/scenario';
import {statusReference} from '@/firebase';

const state = {
    scenarios: [] as Scenario[],
};

const mutations = {
    scenariosMutator(state: any, scenarios: Scenario[]) {
        state.scenarios = [...scenarios];
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
    }
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations
};
