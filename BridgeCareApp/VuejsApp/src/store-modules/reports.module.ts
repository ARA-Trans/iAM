import DetailedReportService from '@/services/detailed-report.service';
import SummaryReportService from '@/services/summary-report.service';
import {AxiosResponse} from 'axios';
import {http200Response} from '@/shared/utils/http-codes-regex-utils';
import {hasValue} from '@/shared/utils/has-value-util';

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
                 await new DetailedReportService().getDetailedReport(payload.networkId, payload.simulationId)
                     .then((response: AxiosResponse<Blob>) => {
                         if (http200Response.test(response.status.toString())) {
                             commit('reportsBlobMutator', new Blob([response.data]));
                             commit('currentReportNameMutator', 'Detailed report.xlsx');
                         } else {
                             const statusText: string = hasValue(response.statusText)
                                 ? `: HTTP Code ${response.status} => ${response.statusText}`
                                 : `: HTTP Code ${response.status}`;
                             dispatch('setErrorMessage', {message: `Detailed Report Download Failed${statusText}`});
                         }
                    });
                break;
            }
            case 'Summary report': {
                await new SummaryReportService()
                    .getSummaryReport(payload.networkId, payload.simulationId, payload.networkName, payload.simulationName)
                    .then((response: AxiosResponse<Blob>) => {
                        if (http200Response.test(response.status.toString())) {
                            commit('reportsBlobMutator', new Blob([response.data]));
                            commit('currentReportNameMutator', 'Summary report.xlsx');
                        } else {
                            const statusText: string = hasValue(response.statusText)
                                ? `: HTTP Code ${response.status} => ${response.statusText}`
                                : `: HTTP Code ${response.status}`;
                            dispatch('setErrorMessage', {message: `Summary Report Download Failed${statusText}`});
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