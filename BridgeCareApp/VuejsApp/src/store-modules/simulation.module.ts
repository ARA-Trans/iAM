import {Simulation} from '@/shared/models/iAM/simulation';
import SimulationService from '../services/simulation.service';
import { statusReference } from '../firebase';
import moment from 'moment';

const state = {
    simulations: [] as Simulation[]
};

const mutations = {
    simulationsMutator(state: any, simulations: Simulation[]) {
        state.simulations = simulations;
    }
};

const actions = {
    async getSimulations({commit}: any, payload: any) {
        await new SimulationService().getSimulations(payload.networkId)
            .then((simulations: Simulation[]) => commit('simulationsMutator', simulations))
            .catch((error: any) => console.log(error));
    },
    runSimulation({ commit }: any, payload: any) {
        statusReference.once('value', (snapshot) => {
            if (snapshot.hasChild(payload.networkId.toString() + '_' + payload.simulationId.toString())) {
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
                        new SimulationService().runSimulation(payload.networkId, payload.networkName, payload.simulationId, payload.simulationName)
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
                        new SimulationService().runSimulation(payload.networkId, payload.networkName, payload.simulationId, payload.simulationName)
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
