import {Scenario} from '@/shared/models/iAM/scenario';
import ScenarioService from '@/services/scenario.service';
import {AxiosResponse} from 'axios';
import {http2XX, setStatusMessage} from '@/shared/utils/http-utils';
import { hasValue } from '@/shared/utils/has-value-util';
import { clone, append, any, propEq, findIndex, remove } from 'ramda';

const state = {
    scenarios: [] as Scenario[],
    benefitAttributes: [] as string[]
};

const mutations = {
    scenariosMutator(state: any, scenarios: Scenario[]) {
        state.scenarios = clone(scenarios);
    },
    createdScenarioMutator(state: any, createdScenario: Scenario) {
        state.scenarios = append(createdScenario, state.scenarios);
    },
    updatedScenarioMutator(state: any, updatedScenario: Scenario) {
        if (any(propEq('simulationId', updatedScenario.simulationId), state.scenarios)) {
            const scenarios: Scenario[] = clone(state.scenarios);
            const index: number = findIndex(propEq('simulationId', updatedScenario.simulationId), scenarios);
            scenarios[index] = clone(updatedScenario);
            state.scenarios = scenarios;
        }
    },
    removeScenarioMutator(state: any, simulationId: number) {
        if (any(propEq('simulationId', simulationId), state.scenarios)) {
            const scenarios: Scenario[] = clone(state.scenarios);
            const index: number = findIndex(propEq('simulationId', simulationId), scenarios);
            state.scenarios = remove(index, 1, scenarios);
        }
    }
};

const actions = {
    async getUserScenarios({dispatch, commit}: any, payload: any) {
        return await ScenarioService.getUserScenarios(payload.userId)
            .then((response: AxiosResponse<Scenario[]>) => {
                if (hasValue(response) && http2XX.test(response.status.toString())) {
                    commit('scenariosMutator', response.data);
                } else {
                    dispatch('setErrorMessage', {message: `Failed to get scenarios${setStatusMessage(response)}`});
                }
            });
    },
    async createScenario({dispatch, commit}: any, payload: any) {
        return await ScenarioService.createScenario(payload.createScenarioData, payload.userId)
            .then((response: AxiosResponse<Scenario>) => {
                if (hasValue(response) && http2XX.test(response.status.toString())) {
                    commit('createdScenarioMutator', response.data);
                    dispatch('setSuccessMessage', {message: 'Successfully created scenario'});
                } else {
                    dispatch('setErrorMessage', {message: `Failed to create scenario${setStatusMessage(response)}`});
                }
            });
    },
    async runSimulation({dispatch, commit}: any, payload: any) {
        await ScenarioService.runScenarioSimulation(payload.selectedScenario, payload.userId)
            .then((response: AxiosResponse<Scenario>) => {
                if (hasValue(response) && http2XX.test(response.status.toString())) {
                    dispatch('setSuccessMessage', {message: 'Simulation started'});
                } else {
                    dispatch('setErrorMessage', {message: `Failed to start simulation${setStatusMessage(response)}`});
                }
            });
    },
    async deleteScenario({ dispatch, commit }: any, payload: any) {
        return await ScenarioService.deleteScenario(payload.scenarioId)
            .then((response: AxiosResponse<number>) => {
                if (hasValue(response) && http2XX.test(response.status.toString())) {
                    commit('removeScenarioMutator', response.data);
                    dispatch('setSuccessMessage', { message: 'Successfully deleted scenario' });
                } else {
                    dispatch('setErrorMessage', { message: `Failed to delete scenario${setStatusMessage(response)}` });
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
