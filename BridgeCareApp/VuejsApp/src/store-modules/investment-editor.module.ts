import {emptyInvestmentLibrary, InvestmentLibrary} from '@/shared/models/iAM/investment';
import InvestmentEditorService from '@/services/investment-editor.service';
import {clone, any, propEq, append, findIndex, equals, update, reject} from 'ramda';
import {AxiosResponse} from 'axios';
import {hasValue} from '@/shared/utils/has-value-util';
import {convertFromMongoToVue} from '@/shared/utils/mongo-model-conversion-utils';
import {http2XX} from '@/shared/utils/http-utils';

const state = {
    investmentLibraries: [] as InvestmentLibrary[],
    scenarioInvestmentLibrary: clone(emptyInvestmentLibrary) as InvestmentLibrary,
    selectedInvestmentLibrary: clone(emptyInvestmentLibrary) as InvestmentLibrary
};

const mutations = {
    investmentLibrariesMutator(state: any, investmentLibraries: InvestmentLibrary[]) {
        state.investmentLibraries = clone(investmentLibraries);
    },
    selectedInvestmentLibraryMutator(state: any, selectedInvestmentLibrary: InvestmentLibrary) {
        state.selectedInvestmentLibrary = clone(selectedInvestmentLibrary);
    },
    createdInvestmentLibraryMutator(state: any, createdInvestmentLibrary: InvestmentLibrary) {
        state.investmentLibraries = append(createdInvestmentLibrary, state.investmentLibraries);
    },
    updatedInvestmentLibraryMutator(state: any, updatedInvestmentLibrary: InvestmentLibrary) {
        state.investmentLibraries = update(
            findIndex(propEq('id', updatedInvestmentLibrary.id), state.investmentLibraries),
            updatedInvestmentLibrary,
            state.investmentLibraries
        );
    },
    scenarioInvestmentLibraryMutator(state: any, scenarioInvestmentLibrary: InvestmentLibrary) {
        state.scenarioInvestmentLibrary = clone(scenarioInvestmentLibrary);
    },
    deletedInvestmentLibraryMutator(state: any, deletedInvestmentLibraryId: string) {
        if (any(propEq('id', deletedInvestmentLibraryId), state.investmentLibraries)) {
            state.investmentLibraries = reject(
                (library: InvestmentLibrary) => deletedInvestmentLibraryId === library.id,
                state.investmentLibraries
            );
        }
    }
};

const actions = {
    selectInvestmentLibrary({commit}: any, payload: any) {
        commit('selectedInvestmentLibraryMutator', payload.selectedInvestmentLibrary);
    },
    async getInvestmentLibraries({commit}: any) {
        await InvestmentEditorService.getInvestmentLibraries()
            .then((response: AxiosResponse<any[]>) => {
                if (hasValue(response, 'data')) {
                    const investmentLibraries: InvestmentLibrary[] = response.data
                        .map((data: any) => convertFromMongoToVue(data));
                    commit('investmentLibrariesMutator', investmentLibraries);
                }
            });
    },
    async createInvestmentLibrary({dispatch, commit}: any, payload: any) {
        await InvestmentEditorService.createInvestmentLibrary(payload.createdInvestmentLibrary)
            .then((response: AxiosResponse<any>) => {
                if (hasValue(response, 'data')) {
                    const createdInvestmentLibrary: InvestmentLibrary = convertFromMongoToVue(response.data);
                    commit('createdInvestmentLibraryMutator', createdInvestmentLibrary);
                    dispatch('setSuccessMessage', {message: 'Successfully created investment library'});
                }
            });
    },
    async updateInvestmentLibrary({dispatch, commit}: any, payload: any) {
        await InvestmentEditorService.updateInvestmentLibrary(payload.updatedInvestmentLibrary)
            .then((response: AxiosResponse<any>) => {
                if (hasValue(response, 'data')) {
                    const updatedInvestmentLibrary: InvestmentLibrary = convertFromMongoToVue(response.data);
                    commit('updatedInvestmentLibraryMutator', updatedInvestmentLibrary);
                    commit('selectedInvestmentLibraryMutator', updatedInvestmentLibrary.id);
                    dispatch('setSuccessMessage', {message: 'Successfully updated investment library'});
                }
            });
    },
    async deleteInvestmentLibrary({dispatch, commit}: any, payload: any) {
        await InvestmentEditorService.deleteInvestmentLibrary(payload.investmentLibrary)
            .then((response: AxiosResponse<any>) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                commit('deletedInvestmentLibraryMutator', payload.investmentLibrary.id);
                dispatch('setSuccessMessage', {message: 'Successfully deleted investment library'});
                }
            });
    },
    async getScenarioInvestmentLibrary({dispatch, commit}: any, payload: any) {
        if (payload.selectedScenarioId !== '0') {
            await InvestmentEditorService.getScenarioInvestmentLibrary(payload.selectedScenarioId)
                .then((response: AxiosResponse<InvestmentLibrary>) => {
                    if (hasValue(response, 'data')) {
                        commit('scenarioInvestmentLibraryMutator', response.data);
                        commit('updatedSelectedInvestmentLibraryMutator', response.data);
                    }
                });
        } else {
            commit('scenarioInvestmentLibraryMutator', emptyInvestmentLibrary);
            commit('updatedSelectedInvestmentLibraryMutator', emptyInvestmentLibrary);
        }
    },
    async saveScenarioInvestmentLibrary({dispatch, commit}: any, payload: any) {
        await InvestmentEditorService.saveScenarioInvestmentLibrary(payload.saveScenarioInvestmentLibraryData)
            .then((response: AxiosResponse<InvestmentLibrary>) => {
                if (hasValue(response, 'data')) {
                    commit('scenarioInvestmentLibraryMutator', response.data);
                    dispatch('setSuccessMessage', {message: 'Successfully saved scenario investment library'});
                }
            });
    },
    async socket_investmentLibrary({dispatch, state, commit}: any, payload: any) {
        if (hasValue(payload, 'operationType') && hasValue(payload, 'fullDocument')) {
            const investmentLibrary: InvestmentLibrary = convertFromMongoToVue(payload.fullDocument);
            switch (payload.operationType) {
                case 'update':
                case 'replace':
                    commit('updatedInvestmentLibraryMutator', investmentLibrary);
                    if (state.selectedInvestmentLibrary.id === investmentLibrary.id &&
                        !equals(state.selectedInvestmentLibrary, investmentLibrary)) {
                        commit('selectedInvestmentLibraryMutator', investmentLibrary);
                        dispatch('setInfoMessage',
                            {message: `Investment library '${investmentLibrary.name}' has been changed from another source`}
                        );
                    }
                    break;
                case 'insert':
                    if (!any(propEq('id', investmentLibrary.id), state.investmentLibraries)) {
                        commit('createdInvestmentLibraryMutator', investmentLibrary);
                        dispatch('setInfoMessage',
                            {message: `Investment library '${investmentLibrary.name}' has been created from another source`}
                        );
                    }
            }
        } else if (hasValue(payload, 'operationType') && payload.operationType === 'delete') {
            if (any(propEq('id', payload.documentKey._id), state.investmentLibraries)) {
                commit('deletedInvestmentLibraryMutator', payload.documentKey._id);
                dispatch('setInfoMessage',
                    {message: `An investment library has been deleted from another source`}
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
