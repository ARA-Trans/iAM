import {Deficient, DeficientLibrary, emptyDeficientLibrary} from '@/shared/models/iAM/deficient';
import {clone, append, any, propEq, update, findIndex, equals, reject} from 'ramda';
import DeficientService from '@/services/deficient.service';
import {AxiosResponse} from 'axios';
import {hasValue} from '@/shared/utils/has-value-util';
import {http2XX} from '@/shared/utils/http-utils';
import {convertFromMongoToVue} from '@/shared/utils/mongo-model-conversion-utils';

const state = {
    deficientLibraries: [] as DeficientLibrary[],
    selectedDeficientLibrary: clone(emptyDeficientLibrary) as DeficientLibrary,
    scenarioDeficientLibrary: clone(emptyDeficientLibrary) as DeficientLibrary
};

const mutations = {
    deficientLibrariesMutator(state: any, deficientLibraries: DeficientLibrary[]) {
        state.deficientLibraries = clone(deficientLibraries);
    },
    selectedDeficientLibraryMutator(state: any, selectedDeficientLibrary: DeficientLibrary) {
        state.selectedDeficientLibrary = clone(selectedDeficientLibrary);
    },
    createdDeficientLibraryMutator(state: any, createdDeficientLibrary: DeficientLibrary) {
        state.deficientLibraries = append(createdDeficientLibrary, state.deficientLibraries);
    },
    updatedDeficientLibraryMutator(state: any, updatedDeficientLibrary: DeficientLibrary) {
        if (any(propEq('id', updatedDeficientLibrary.id), state.deficientLibraries)) {
            state.deficientLibraries = update(
                findIndex(propEq('id', updatedDeficientLibrary.id), state.deficientLibraries),
                updatedDeficientLibrary,
                state.deficientLibraries
            );
        }
    },
    deletedDeficientLibraryMutator(state: any, deletedDeficientLibraryId: string) {
        if (any(propEq('id', deletedDeficientLibraryId), state.deficientLibraries)) {
            state.deficientLibraries = reject(
                (library: DeficientLibrary) => deletedDeficientLibraryId === library.id,
                state.deficientLibraries
            );
        }
    },
    scenarioDeficientLibraryMutator(state: any, scenarioDeficientLibrary: DeficientLibrary) {
        state.scenarioDeficientLibrary = clone(scenarioDeficientLibrary);
    }
};

const actions = {
    selectDeficientLibrary({commit}: any, payload: any) {
        commit('selectedDeficientLibraryMutator', payload.selectedDeficientLibrary);
    },
    async getDeficientLibraries({commit}: any) {
        await DeficientService.getDeficientLibraries().then((response: AxiosResponse<any[]>) => {
            if (hasValue(response, 'data')) {
                const deficientLibraries: DeficientLibrary[] = response.data
                    .map((data: any) => convertFromMongoToVue(data));
                commit('deficientLibrariesMutator', deficientLibraries);
            }
        });
    },
    async createDeficientLibrary({dispatch, commit}: any, payload: any) {
        await DeficientService.createDeficientLibrary(payload.createdDeficientLibrary)
            .then((response: AxiosResponse<any>) => {
                if (hasValue(response, 'data')) {
                    const createdDeficientLibrary: DeficientLibrary = convertFromMongoToVue(response.data);
                    commit('createdDeficientLibraryMutator', createdDeficientLibrary);
                    commit('selectedDeficientLibraryMutator', createdDeficientLibrary);
                    dispatch('setSuccessMessage', {message: 'Successfully created deficient library'});
                }
            });
    },
    async updateDeficientLibrary({dispatch, commit}: any, payload: any) {
        await DeficientService.updateDeficientLibrary(payload.updatedDeficientLibrary)
            .then((response: AxiosResponse<any>) => {
                if (hasValue(response, 'data')) {
                    const updatedDeficientLibrary: DeficientLibrary = convertFromMongoToVue(response.data);
                    commit('updatedDeficientLibraryMutator', updatedDeficientLibrary);
                    commit('selectedDeficientLibraryMutator', updatedDeficientLibrary);
                    dispatch('setSuccessMessage', {message: 'Successfully updated deficient library'});
                }
            });
    },
    async deleteDeficientLibrary({dispatch, commit}: any, payload: any) {
        await DeficientService.deleteDeficientLibrary(payload.deficientLibrary)
            .then((response: AxiosResponse<any>) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                commit('deletedDeficientLibraryMutator', payload.deficientLibrary.id);
                dispatch('setSuccessMessage', {message: 'Successfully deleted deficient library'});
                }
            });
    },
    async getScenarioDeficientLibrary({commit}: any, payload: any) {
        await DeficientService.getScenarioDeficientLibrary(payload.selectedScenarioId)
            .then((response: AxiosResponse<Deficient[]>) => {
                if (hasValue(response, 'data')) {
                    commit('scenarioDeficientLibraryMutator', response.data);
                    commit('selectedDeficientLibraryMutator', response.data);
                }
            });
    },
    async saveScenarioDeficientLibrary({dispatch, commit}: any, payload: any) {
        await DeficientService.saveScenarioDeficientLibrary(payload.saveScenarioDeficientLibraryData, payload.objectIdMOngoDBForScenario)
            .then((response: AxiosResponse<Deficient[]>) => {
                if (hasValue(response, 'data')) {
                    commit('scenarioDeficientLibraryMutator', response.data);
                    dispatch('setSuccessMessage', {message: 'Successfully saved scenario deficient library'});
                }
            });
    },
    async socket_deficientLibrary({dispatch, state, commit}: any, payload: any) {
        if (hasValue(payload, 'operationType') && hasValue(payload, 'fullDocument')) {
            const deficientLibrary: DeficientLibrary = convertFromMongoToVue(payload.fullDocument);
            switch (payload.operationType) {
                case 'update':
                case 'replace':
                    commit('updatedDeficientLibraryMutator', deficientLibrary);
                    if (state.selectedDeficientLibrary.id === deficientLibrary.id &&
                        !equals(state.selectedDeficientLibrary, deficientLibrary)) {
                        commit('selectedDeficientLibraryMutator', deficientLibrary);
                        dispatch('setInfoMessage',
                            {message: `Deficient library '${deficientLibrary.name}' has been changed from another source`}
                        );
                    }
                    break;
                case 'insert':
                    if (!any(propEq('id', deficientLibrary.id), state.deficientLibraries)) {
                        commit('createdDeficientLibraryMutator', deficientLibrary);
                        dispatch('setInfoMessage',
                            {message: `Deficient library '${deficientLibrary.name}' has been created from another source`}
                        );
                    }
            }
        } else if (hasValue(payload, 'operationType') && payload.operationType === 'delete') {
            if (any(propEq('id', payload.documentKey._id), state.deficientLibraries)) {
                commit('deletedDeficientLibraryMutator', payload.documentKey._id);
                dispatch('setInfoMessage',
                    {message: `A deficient library has been deleted from another source`}
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
