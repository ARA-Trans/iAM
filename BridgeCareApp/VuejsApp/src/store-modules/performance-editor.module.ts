import {emptyPerformanceLibrary, PerformanceLibrary} from '@/shared/models/iAM/performance';
import {clone, append, any, propEq, findIndex, equals, update, reject} from 'ramda';
import PerformanceEditorService from '@/services/performance-editor.service';
import {AxiosResponse} from 'axios';
import {hasValue} from '@/shared/utils/has-value-util';
import {convertFromMongoToVue} from '@/shared/utils/mongo-model-conversion-utils';
import {http2XX} from '@/shared/utils/http-utils';

const state = {
    performanceLibraries: [] as PerformanceLibrary[],
    scenarioPerformanceLibrary: clone(emptyPerformanceLibrary) as PerformanceLibrary,
    selectedPerformanceLibrary: clone(emptyPerformanceLibrary) as PerformanceLibrary
};

const mutations = {
    performanceLibrariesMutator(state: any, performanceLibraries: PerformanceLibrary[]) {
        state.performanceLibraries = clone(performanceLibraries);
    },
    selectedPerformanceLibraryMutator(state: any, selectedPerformanceLibrary: PerformanceLibrary) {
       state.selectedPerformanceLibrary = clone(selectedPerformanceLibrary);
    },
    createdPerformanceLibraryMutator(state: any, createdPerformanceLibrary: PerformanceLibrary) {
        state.performanceLibraries = append(createdPerformanceLibrary, state.performanceLibraries);
    },
    updatedPerformanceLibraryMutator(state: any, updatedPerformanceLibrary: PerformanceLibrary) {
        state.performanceLibraries = update(
            findIndex(propEq('id', updatedPerformanceLibrary.id), state.performanceLibraries),
            updatedPerformanceLibrary,
            state.performanceLibraries
        );
    },
    deletedPerformanceLibraryMutator(state: any, deletedPerformanceLibraryId: string) {
        if (any(propEq('id', deletedPerformanceLibraryId), state.performanceLibraries)) {
            state.performanceLibraries = reject(
                (library: PerformanceLibrary) => deletedPerformanceLibraryId === library.id,
                state.performanceLibraries
            );
        }
    },
    scenarioPerformanceLibraryMutator(state: any, scenarioPerformanceLibrary: PerformanceLibrary) {
        state.scenarioPerformanceLibrary = clone(scenarioPerformanceLibrary);
    }
};

const actions = {
    selectPerformanceLibrary({commit}: any, payload: any) {
        commit('selectedPerformanceLibraryMutator', payload.selectedPerformanceLibrary);
    },
    async getPerformanceLibraries({commit}: any) {
        await PerformanceEditorService.getPerformanceLibraries()
            .then((response: AxiosResponse<any[]>) => {
                if (hasValue(response, 'data')) {
                    const performanceLibraries: PerformanceLibrary[] = response.data
                        .map((data: any) => convertFromMongoToVue(data));
                    commit('performanceLibrariesMutator', performanceLibraries);
                }
            });
    },
    async createPerformanceLibrary({dispatch, commit}: any, payload: any) {
        await PerformanceEditorService.createPerformanceLibrary(payload.createdPerformanceLibrary)
            .then((response: AxiosResponse<any>) => {
                if (hasValue(response, 'data')) {
                    const createdPerformanceLibrary: PerformanceLibrary = convertFromMongoToVue(response.data);
                    commit('createdPerformanceLibraryMutator', createdPerformanceLibrary);
                    commit('selectedPerformanceLibraryMutator', createdPerformanceLibrary);
                    dispatch('setSuccessMessage', {message: 'Successfully created performance library'});
                }
            });
    },
    async updatePerformanceLibrary({dispatch, commit}: any, payload: any) {
        await PerformanceEditorService.updatePerformanceLibrary(payload.updatedPerformanceLibrary)
            .then((response: AxiosResponse<any>) => {
                if (hasValue(response, 'data')) {
                    const updatedPerformanceLibrary: PerformanceLibrary = convertFromMongoToVue(response.data);
                    commit('updatedPerformanceLibraryMutator', updatedPerformanceLibrary);
                    commit('selectedPerformanceLibraryMutator', updatedPerformanceLibrary);
                    dispatch('setSuccessMessage', {message: 'Successfully updated performance library'});
                }
            });
    },
    async deletePerformanceLibrary({dispatch, commit}: any, payload: any) {
        await PerformanceEditorService.deletePerformanceLibrary(payload.performanceLibrary)
            .then((response: AxiosResponse<any>) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                commit('deletedPerformanceLibraryMutator', payload.performanceLibrary.id);
                dispatch('setSuccessMessage', {message: 'Successfully deleted performance library'});
                }
            });
    },
    async getScenarioPerformanceLibrary({commit}: any, payload: any) {
        await PerformanceEditorService.getScenarioPerformanceLibrary(payload.selectedScenarioId)
            .then((response: AxiosResponse<PerformanceLibrary>) => {
                if (hasValue(response, 'data')) {
                    commit('scenarioPerformanceLibraryMutator', response.data);
                    commit('selectedPerformanceLibraryMutator', response.data);
                }
            });
    },
    async saveScenarioPerformanceLibrary({dispatch, commit}: any, payload: any) {
        await PerformanceEditorService.saveScenarioPerformanceLibrary(payload.saveScenarioPerformanceLibraryData, payload.objectIdMOngoDBForScenario)
            .then((response: AxiosResponse<PerformanceLibrary>) => {
                if (hasValue(response, 'data')) {
                    commit('scenarioPerformanceLibraryMutator', response.data);
                    dispatch('setSuccessMessage', {message: 'Successfully saved scenario performance library'});
                }
            });
    },
    async socket_performanceLibrary({dispatch, state, commit}: any, payload: any) {
        if (hasValue(payload, 'operationType') && hasValue(payload, 'fullDocument')) {
            const performanceLibrary: PerformanceLibrary = convertFromMongoToVue(payload.fullDocument);
            switch (payload.operationType) {
                case 'update':
                case 'replace':
                    commit('updatedPerformanceLibraryMutator', performanceLibrary);
                    if (state.selectedPerformanceLibrary.id === performanceLibrary.id &&
                        !equals(state.selectedPerformanceLibrary, performanceLibrary)) {
                        commit('selectedPerformanceLibraryMutator', performanceLibrary);
                        dispatch('setInfoMessage',
                            {message: `Performance library '${performanceLibrary.name}' has been changed from another source`}
                        );
                    }
                    break;
                case 'insert':
                    if (!any(propEq('id', performanceLibrary.id), state.performanceLibraries)) {
                        commit('createdPerformanceLibraryMutator', performanceLibrary);
                        dispatch('setInfoMessage',
                            {message: `Performance library '${performanceLibrary.name}' has been created from another source`}
                        );
                    }
            }
        } else if (hasValue(payload, 'operationType') && payload.operationType === 'delete') {
            if (any(propEq('id', payload.documentKey._id), state.performanceLibraries)) {
                commit('deletedPerformanceLibraryMutator', payload.documentKey._id);
                dispatch('setInfoMessage',
                    {message: `A performance library has been deleted from another source`}
                );
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
