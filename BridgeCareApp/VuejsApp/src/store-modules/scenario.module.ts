import {Analysis, emptyAnalysis, Scenario} from '@/shared/models/iAM/scenario';
import ScenarioService from '@/services/scenario.service';
import {AxiosResponse} from 'axios';
import {clone, any, propEq, findIndex, remove} from 'ramda';
import {hasValue} from '@/shared/utils/has-value-util';
import {http2XX} from '@/shared/utils/http-utils';
import prepend from 'ramda/es/prepend';
import AnalysisEditorService from '@/services/analysis-editor.service';
import ReportsService from '@/services/reports.service';

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
    benefitAttributes: [] as string[],
    selectedScenarioName: '',
    analysis: clone(emptyAnalysis) as Analysis,
    missingSummaryReportAttributes: [] as string[]
};

const mutations = {
    scenariosMutator(state: any, scenarios: Scenario[]) {
        state.scenarios = clone(scenarios);
    },
    createdScenarioMutator(state: any, createdScenario: Scenario) {
        state.scenarios = prepend(createdScenario, state.scenarios);
    },
    updatedScenarioMutator(state: any, updatedScenario: Scenario) {
        if (any(propEq('id', updatedScenario.id), state.scenarios)) {
            const scenarios: Scenario[] = clone(state.scenarios);
            const index: number = findIndex(propEq('id', updatedScenario.id), scenarios);
            scenarios[index] = clone(updatedScenario);
            state.scenarios = scenarios;
        }
    },
    removeScenarioMutator(state: any, identifiers: any) {
        if (any(propEq('simulationId', identifiers.id), state.scenarios)) {
            const index: number = findIndex(propEq('simulationId', identifiers.id), state.scenarios);
            state.scenarios = remove(index, 1, state.scenarios);
        } else if (any(propEq('id', identifiers.mongoId), state.scenarios)) {
            const index: number = findIndex(propEq('id', identifiers.mongoId), state.scenarios);
            state.scenarios = remove(index, 1, state.scenarios);
        }
    },
    selectedScenarioNameMutator(state: any, selectedScenarioName: string) {
        state.selectedScenarioName = selectedScenarioName;
    },
    analysisMutator(state: any, analysis: Analysis) {
        state.analysis = clone(analysis);
    },
    missingSummaryReportAttributesMutator(state: any, missingAttributes: string[]) {
        state.missingSummaryReportAttributes = clone(missingAttributes);
    }
};

const actions = {
    setSelectedScenarioName({commit}: any, payload: any) {
        commit('selectedScenarioNameMutator', payload.selectedScenarioName);
    },
    async getMongoScenarios({commit}: any) {
        return await ScenarioService.getMongoScenarios()
            .then((response: AxiosResponse<any[]>) => {
                if (hasValue(response, 'data')) {
                    const scenarios: Scenario[] = response.data
                        .map((data: any) => {
                            return convertFromMongoToVueModel(data);
                        });
                    commit('scenariosMutator', scenarios);
                }
            });
    },
    async getLegacyScenarios({ commit }: any, payload: any) {
        return await ScenarioService.getLegacyScenarios()
            .then((response: AxiosResponse<Scenario[]>) => {
                if (hasValue(response, 'data')) {
                    const scenarios: Scenario[] = response.data
                        .map((data: any) => {
                            return convertFromMongoToVueModel(data);
                        });
                    // Socket.io will update the list.
                }
            });
    },
    async runSimulation({dispatch, commit}: any, payload: any) {
        await ScenarioService.runScenarioSimulation(payload.selectedScenario, payload.userId)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                    dispatch('setSuccessMessage', {message: 'Simulation started'});
                }
            });
    },
    async createScenario({dispatch, commit}: any, payload: any) {
        return await ScenarioService.createScenario(payload.createScenarioData, payload.userId)
            .then((response: AxiosResponse<any>) => {
                if (hasValue(response, 'data')) {
                    const createdScenario: Scenario = convertFromMongoToVueModel(response.data);
                    commit('createdScenarioMutator', createdScenario);
                    dispatch('setSuccessMessage', {message: 'Successfully created scenario'});
                }
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
    async deleteScenario({ dispatch, state, commit }: any, payload: any) {
        return await ScenarioService.deleteScenario(payload.simulationId, payload.scenarioId)
            .then((response: AxiosResponse<any>) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                    if (any(propEq('simulationId', payload.simulationId), state.scenarios) ||
                        any(propEq('id', payload.scenarioId), state.scenarios)) {
                        commit('removeScenarioMutator', {id: payload.simulationId, mongoId: payload.scenarioId});
                        dispatch('setSuccessMessage', {message: 'Successfully deleted scenario'});
                    }
                }
            });
    },
    async getScenarioAnalysis({commit}: any, payload: any) {
        await AnalysisEditorService.getScenarioAnalysisData(payload.selectedScenarioId)
            .then((response: AxiosResponse<any>) => {
                commit('analysisMutator', hasValue(response, 'data') ? response.data : emptyAnalysis);
            });
    },
    async saveScenarioAnalysis({dispatch, commit}: any, payload: any) {
        await AnalysisEditorService.saveScenarioAnalysisData(payload.scenarioAnalysisData)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                    commit('analysisMutator', payload.scenarioAnalysisData);
                    dispatch('setSuccessMessage', {message: 'Successfully saved scenario analysis'});
                }
            });
    },
    async getSummaryReportMissingAttributes({commit}: any, payload: any) {
        await ReportsService.getSummaryReportMissingAttributes(payload.selectedScenarioId, payload.selectedNetworkId)
            .then((response: AxiosResponse<string[]>) => {
                if (hasValue(response, 'data')) {
                    commit('missingSummaryReportAttributesMutator', response.data);
                }
            });
    },
    async clearSummaryReportMissingAttributes({commit}: any) {
        commit('missingSummaryReportAttributesMutator', '');
    },
    async socket_scenarioStatus({ dispatch, state, commit }: any, payload: any) {
        if (hasValue(payload, 'operationType')) {
            switch (payload.operationType) {
                case 'update':
                case 'replace':
                    if (hasValue(payload, 'fullDocument')) {
                        const updatedScenario: Scenario = convertFromMongoToVueModel(payload.fullDocument);
                        commit('updatedScenarioMutator', updatedScenario);
                    }
                    break;
                case 'insert':
                    if (hasValue(payload, 'fullDocument')) {
                        const createdScenario: Scenario = convertFromMongoToVueModel(payload.fullDocument);
                        if (!any(propEq('id', createdScenario.id), state.scenarios)) {
                            commit('createdScenarioMutator', createdScenario);
                            dispatch('setInfoMessage', {message: 'New scenario has been inserted from another source'});
                        }
                    }
                    break;
                case 'delete':
                    if (hasValue(payload, 'documentKey')) {
                        const scenarioId: string = payload.documentKey._id as string;
                        if (any(propEq('id', scenarioId), state.scenarios)) {
                            const deletedScenario: Scenario = state.scenarios
                                .find((scenario: Scenario) => scenario.id === scenarioId) as Scenario;
                            commit('removeScenarioMutator', {id: deletedScenario.simulationId, mongoId: scenarioId});
                            dispatch('setInfoMessage',
                                {message: `Scenario '${deletedScenario.simulationName}' was deleted from another source`}
                            );
                        }
                    }
                    break;
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
