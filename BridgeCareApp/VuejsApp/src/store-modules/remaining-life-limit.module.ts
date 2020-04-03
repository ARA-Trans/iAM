import {emptyRemainingLifeLimitLibrary, RemainingLifeLimitLibrary} from '@/shared/models/iAM/remaining-life-limit';
import {any, append, clone, equals, find, findIndex, propEq, reject, update} from 'ramda';
import RemainingLifeLimitService from '@/services/remaining-life-limit.service';
import {AxiosResponse} from 'axios';
import {hasValue} from '@/shared/utils/has-value-util';
import {convertFromMongoToVue} from '@/shared/utils/mongo-model-conversion-utils';
import {http2XX} from '@/shared/utils/http-utils';

const state = {
    remainingLifeLimitLibraries: [] as RemainingLifeLimitLibrary[],
    scenarioRemainingLifeLimitLibrary: clone(emptyRemainingLifeLimitLibrary) as RemainingLifeLimitLibrary,
    selectedRemainingLifeLimitLibrary: clone(emptyRemainingLifeLimitLibrary) as RemainingLifeLimitLibrary
};

const mutations = {
    /**
     * Sets state.remainingLifeLimitLibraries to a copy of remainingLifeLimitLibraries
     * @param state App state
     * @param remainingLifeLimitLibraries List of remaining life limit libraries
     */
    remainingLifeLimitLibrariesMutator(state: any, remainingLifeLimitLibraries: RemainingLifeLimitLibrary[]) {
        state.remainingLifeLimitLibraries = clone(remainingLifeLimitLibraries);
    },
    /**
     * Sets state.selectedRemainingLifeLimitLibrary with selectedRemainingLifeLimitLibrary
     * state.remainingLifeLimitLibraries
     * @param state App state
     * @param selectedRemainingLifeLimitLibrary Selected remaining life limit library
     */
    selectedRemainingLifeLimitLibraryMutator(state: any, libraryId: string) {
        if (any(propEq('id', libraryId), state.remainingLifeLimitLibraries)) {
            state.selectedRemainingLifeLimitLibrary = find(propEq('id', libraryId), state.remainingLifeLimitLibraries);
        } else if (state.scenarioRemainingLifeLimitLibrary.id === libraryId) {
            state.selectedRemainingLifeLimitLibrary = clone(state.scenarioRemainingLifeLimitLibrary);
        } else {
            state.selectedRemainingLifeLimitLibrary = clone(emptyRemainingLifeLimitLibrary);
        }
    },
    /**
     * Appends createdRemainingLifeLimitLibrary to state.remainingLifeLimitLibraries
     * @param state App state
     * @param createdRemainingLifeLimitLibrary Created remaining life limit library
     */
    createdRemainingLifeLimitLibraryMutator(state: any, createdRemainingLifeLimitLibrary: RemainingLifeLimitLibrary) {
        state.remainingLifeLimitLibraries = append(createdRemainingLifeLimitLibrary, state.remainingLifeLimitLibraries);
    },
    /**
     * Updates a remaining life limit library in state.remainingLifeLimitLibraries with updatedRemainingLifeLimitLibrary
     * @param state App state
     * @param updatedRemainingLifeLimitLibrary Updated remaining life limit library
     */
    updatedRemainingLifeLimitLibraryMutator(state: any, updatedRemainingLifeLimitLibrary: RemainingLifeLimitLibrary) {
        state.remainingLifeLimitLibraries = update(
            findIndex(propEq('id', updatedRemainingLifeLimitLibrary.id), state.remainingLifeLimitLibraries),
            updatedRemainingLifeLimitLibrary,
            state.remainingLifeLimitLibraries
        );
    },
    deletedRemainingLifeLimitLibraryMutator(state: any, deletedRemainingLifeLimitLibraryId: string) {
        if (any(propEq('id', deletedRemainingLifeLimitLibraryId), state.remainingLifeLimitLibraries)) {
            state.remainingLifeLimitLibraries = reject(
                (library: RemainingLifeLimitLibrary) => deletedRemainingLifeLimitLibraryId === library.id,
                state.remainingLifeLimitLibraries
            );
        }
    },
    /**
     * Sets state.scenarioRemainingLifeLimitLibrary with a copy of scenarioRemainingLifeLimitLibrary
     * @param state App state
     * @param scenarioRemainingLifeLimitLibrary Scenario's remaining life limit library
     */
    scenarioRemainingLifeLimitLibraryMutator(state: any, scenarioRemainingLifeLimitLibrary: RemainingLifeLimitLibrary) {
        state.scenarioRemainingLifeLimitLibrary = clone(scenarioRemainingLifeLimitLibrary);
    }
};

const actions = {
    selectRemainingLifeLimitLibrary({commit}: any, payload: any) {
        commit('selectedRemainingLifeLimitLibraryMutator', payload.selectedLibraryId);
    },
    async getRemainingLifeLimitLibraries({commit}: any) {
        await RemainingLifeLimitService.getRemainingLifeLimitLibraries()
            .then((response: AxiosResponse<any[]>) => {
                if (hasValue(response, 'data')) {
                    const remainingLifeLimitLibraries: RemainingLifeLimitLibrary[] = response.data
                        .map((data: any) => convertFromMongoToVue(data));
                    commit('remainingLifeLimitLibrariesMutator', remainingLifeLimitLibraries);
                }
            });
    },
    async createRemainingLifeLimitLibrary({dispatch, commit}: any, payload: any) {
        await RemainingLifeLimitService.createRemainingLifeLimitLibrary(payload.createdRemainingLifeLimitLibrary)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    const createdRemainingLifeLimitLibrary: RemainingLifeLimitLibrary = convertFromMongoToVue(response.data);
                    commit('createdRemainingLifeLimitLibraryMutator', createdRemainingLifeLimitLibrary);
                    commit('selectedRemainingLifeLimitLibraryMutator', createdRemainingLifeLimitLibrary.id);
                    dispatch('setSuccessMessage', {message: 'Successfully created remaining life limit library'});
                }
            });
    },
    async updateRemainingLifeLimitLibrary({dispatch, commit}: any, payload: any) {
        await RemainingLifeLimitService.updateRemainingLifeLimitLibrary(payload.updatedRemainingLifeLimitLibrary)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    const updatedRemainingLifeLimitLibrary: RemainingLifeLimitLibrary = convertFromMongoToVue(response.data);
                    commit('updatedRemainingLifeLimitLibraryMutator', updatedRemainingLifeLimitLibrary);
                    commit('selectedRemainingLifeLimitLibraryMutator', updatedRemainingLifeLimitLibrary.id);
                    dispatch('setSuccessMessage', {message: 'Successfully updated remaining life limit library'});
                }
            });
    },
    async deleteRemainingLifeLimitLibrary({dispatch, commit}: any, payload: any) {
        await RemainingLifeLimitService.deleteRemainingLifeLimitLibrary(payload.remainingLifeLimitLibrary)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                    commit('deletedRemainingLifeLimitLibraryMutator', payload.remainingLifeLimitLibrary.id);
                    dispatch('setSuccessMessage', {message: 'Successfully deleted remaining life limit library'});
                }
            });
    },
    async getScenarioRemainingLifeLimitLibrary({dispatch, commit}: any, payload: any) {
        await RemainingLifeLimitService.getScenarioRemainingLifeLimitLibrary(payload.selectedScenarioId)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    commit('scenarioRemainingLifeLimitLibraryMutator', response.data);
                    commit('selectedRemainingLifeLimitLibraryMutator', response.data.id);
                }
            });
    },
    async saveScenarioRemainingLifeLimitLibrary({dispatch, commit}: any, payload: any) {
        await RemainingLifeLimitService.saveScenarioRemainingLifeLimitLibrary(payload.saveScenarioRemainingLifeLimitLibraryData,
            payload.objectIdMOngoDBForScenario)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    commit('scenarioRemainingLifeLimitLibraryMutator', response.data);
                    dispatch('setSuccessMessage', {message: 'Successfully saved scenario remaining life limit library'});
                }
            });
    },
    async socket_remainingLifeLimitLibrary({dispatch, state, commit}: any, payload: any) {
        if (hasValue(payload, 'operationType')) {
            if (hasValue(payload, 'fullDocument')) {
                const remainingLifeLimitLibrary: RemainingLifeLimitLibrary = convertFromMongoToVue(payload.fullDocument);
                switch (payload.operationType) {
                    case 'update':
                    case 'replace':
                        commit('updatedRemainingLifeLimitLibraryMutator', remainingLifeLimitLibrary);
                        if (state.selectedRemainingLifeLimitLibrary.id === remainingLifeLimitLibrary.id &&
                            !equals(state.selectedRemainingLifeLimitLibrary, remainingLifeLimitLibrary)) {
                            commit('selectedRemainingLifeLimitLibraryMutator', remainingLifeLimitLibrary.id);
                            dispatch('setInfoMessage',
                                {message: `Remaining life limit library '${remainingLifeLimitLibrary.name}' has been changed from another source`}
                            );
                        }
                        break;
                    case 'insert':
                        if (!any(propEq('id', remainingLifeLimitLibrary.id), state.remainingLifeLimitLibraries)) {
                            commit('createdRemainingLifeLimitLibraryMutator', remainingLifeLimitLibrary);
                            dispatch('setInfoMessage',
                                {message: `Remaining life limit library '${remainingLifeLimitLibrary.name}' has been created from another source`}
                            );
                        }
                }
            } else if (hasValue(payload, 'documentKey')) {
                if (any(propEq('id', payload.documentKey._id), state.remainingLifeLimitLibraries)) {
                    const deletedRemainingLifeLimitLibrary: RemainingLifeLimitLibrary = find(
                        propEq('id', payload.documentKey._id), state.remainingLifeLimitLibraries);
                    commit('deletedRemainingLifeLimitLibraryMutator', payload.documentKey._id);

                    if (deletedRemainingLifeLimitLibrary.id === state.selectedRemainingLifeLimitLibrary) {
                        if (!equals(state.scenarioRemainingLifeLimitLibrary, emptyRemainingLifeLimitLibrary)) {
                            commit('selectedRemainingLifeLimitLibraryMutator', state.scenarioRemainingLifeLimitLibrary.id);
                        } else {
                            commit('selectedRemainingLifeLimitLibraryMutator', null);
                        }
                    }

                    dispatch('setInfoMessage',
                        {message: `Remaining life limit library ${deletedRemainingLifeLimitLibrary.name} has been deleted from another source`}
                    );
                }
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
