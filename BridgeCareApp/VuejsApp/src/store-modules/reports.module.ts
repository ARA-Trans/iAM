import {AxiosResponse} from 'axios';
import {http200Response, setStatusMessage} from '@/shared/utils/http-utils';
import ReportsService from '@/services/reports.service';

const state = {
    names: ['Detailed report', 'Summary report'],
    reportsBlob: null,
    currentReportName: ''
};

const mutations = {
    reportsBlobMutator(state: any, reportsBlob: Blob) {
        state.reportsBlob = reportsBlob;
    },
    currentReportNameMutator(state: any, name: string) {
        state.currentReportName = name;
    }
};

const actions = {
    async getReports({dispatch, commit}: any, payload: any) {
        switch (payload.reportName) {
            case 'Detailed report': {
                 await new ReportsService().getDetailedReport(payload.networkId, payload.simulationId)
                     .then((response: AxiosResponse<Blob>) => {
                         if (http200Response.test(response.status.toString())) {
                             commit('reportsBlobMutator', new Blob([response.data]));
                             commit('currentReportNameMutator', 'Detailed report.xlsx');
                         } else {
                             dispatch('setErrorMessage', {message: `Detailed Report Download Failed${setStatusMessage(response)}`});
                         }
                    });
                break;
            }
            case 'Summary report': {
                await new ReportsService()
                    .getSummaryReport(payload.networkId, payload.simulationId, payload.networkName, payload.simulationName)
                    .then((response: AxiosResponse<Blob>) => {
                        if (http200Response.test(response.status.toString())) {
                            commit('reportsBlobMutator', new Blob([response.data]));
                            commit('currentReportNameMutator', 'Summary report.xlsx');
                        } else {
                            dispatch('setErrorMessage', {message: `Summary Report Download Failed${setStatusMessage(response)}`});
                        }
                    });
                break;
            }
        }
    },
    clearReportsBlob({ commit }: any) {
        commit('reportsBlobMutator', null);
    }
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations
};