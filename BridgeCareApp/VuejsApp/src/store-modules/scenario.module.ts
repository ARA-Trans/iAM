import {Scenario} from '@/shared/models/iAM/scenario';
import ScenarioService from '@/services/scenario.service';
import {AxiosResponse} from 'axios';
import {clone, append, any, propEq, findIndex, remove} from 'ramda';
import { hasValue } from '../shared/utils/has-value-util';

const convertFromMongoToVueModel = (data: any) => {
    const scenarios: any = {
        ...data,
        id: data._id
    };
    delete scenarios._id;
    delete scenarios.__v;
    return scenarios as Scenario;
};

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
        if (any(propEq('id', updatedScenario.id), state.scenarios)) {
            const scenarios: Scenario[] = clone(state.scenarios);
            const index: number = findIndex(propEq('id', updatedScenario.id), scenarios);
            scenarios[index] = clone(updatedScenario);
            state.scenarios = scenarios;
        }
    },
    removeScenarioMutator(state: any, documentKey: any) {
        if (any(propEq('id', documentKey), state.scenarios)) {
            const scenarios: Scenario[] = clone(state.scenarios);
            const index: number = findIndex(propEq('id', documentKey), scenarios);
            state.scenarios = remove(index, 1, scenarios);
        }
    }
};

const actions = {
    async getUserScenarios({commit}: any, payload: any) {
        return await ScenarioService.getUserScenarios(payload.userId)
            .then((response: AxiosResponse<Scenario[]>) => {
                const scenarios: Scenario[] = response.data
                    .map((data: any) => {
                        return convertFromMongoToVueModel(data);
                    });
                commit('scenariosMutator', scenarios);
            });
    },
    async getLegacyScenarios({ commit }: any, payload: any) {
        return await ScenarioService.getLegacyScenarios(payload.scenarios)
            .then((response: AxiosResponse<Scenario[]>) => {
                if (hasValue(response)) {
                    const scenarios: Scenario[] = response.data
                        .map((data: any) => {
                            return convertFromMongoToVueModel(data);
                        });
                    commit('scenariosMutator', scenarios);
                }
            });
    },
    async createScenario({dispatch, commit}: any, payload: any) {
        return await ScenarioService.createScenario(payload.createScenarioData, payload.userId)
            .then((response: AxiosResponse<Scenario>) => {
                const createdScenario: Scenario = convertFromMongoToVueModel(response.data);
                commit('createdScenarioMutator', createdScenario);
                dispatch('setSuccessMessage', {message: 'Successfully created scenario'});
            });
    },
    async runSimulation({dispatch, commit}: any, payload: any) {
        await ScenarioService.runScenarioSimulation(payload.selectedScenario, payload.userId)
            .then((response: AxiosResponse<Scenario>) => {
                dispatch('setSuccessMessage', {message: 'Simulation started'});
            });
    },
    async deleteScenario({ dispatch, commit }: any, payload: any) {
        return await ScenarioService.deleteScenario(payload.simulationId, payload.scenarioId)
            .then((response: AxiosResponse<number>) => {
                commit('removeScenarioMutator', response.data);
                dispatch('setSuccessMessage', { message: 'Successfully deleted scenario' });
            });
    },
    async updateScenario({ dispatch, commit }: any, payload: any) {
        return await ScenarioService.updateScenario(payload.updateScenarioData, payload.scenarioId)
            .then((response: AxiosResponse<Scenario>) => {
                const updatedScenario: Scenario = convertFromMongoToVueModel(response.data);
                commit('updatedScenarioMutator', updatedScenario);
                dispatch('setSuccessMessage', { message: 'Successfully updated scenario' });
            });
    },
    async socket_scenarioStatus({ dispatch, state, commit }: any, payload: any) {
        if (payload.operationType == 'update' || payload.operationType == 'replace') {
            const updatedScenario: Scenario = convertFromMongoToVueModel(payload.fullDocument);
            commit('updatedScenarioMutator', updatedScenario);
        }

        if (payload.operationType == 'insert') {
            const createdScenario: Scenario = convertFromMongoToVueModel(payload.fullDocument);
            if (!any(propEq('id', createdScenario.id), state.scenarios)) {
                commit('createdScenarioMutator', createdScenario);
                dispatch('setInfoMessage', {message: 'New scenario has been inserted from another source'});
            }
        }

        if (payload.operationType == 'delete') {
            const deletedScenario: Scenario = convertFromMongoToVueModel(payload.documentKey);
            if (any(propEq('id', deletedScenario.id), state.scenarios)) {
                commit('removeScenarioMutator', deletedScenario.id);
                dispatch('setInfoMessage', {message: 'A scenario has been deleted from another source'});
            }
        }
    }
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations
};
