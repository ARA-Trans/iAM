﻿import {AxiosResponse} from 'axios';
import {http2XX, setStatusMessage} from '@/shared/utils/http-utils';
import ReportsService from '@/services/reports.service';
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
                 await ReportsService.getDetailedReport(payload.networkId, payload.simulationId)
                     .then((response: AxiosResponse<Blob>) => {
                         if (hasValue(response) && http2XX.test(response.status.toString())) {
                             commit('reportsBlobMutator', new Blob([response.data]));
                             commit('currentReportNameMutator', 'DetailedReport.xlsx');
                         } else {
                             dispatch('setErrorMessage', {message: `Detailed Report Download Failed${setStatusMessage(response)}`});
                         }
                    });
                break;
            }
            case 'Summary report': {
                await ReportsService
                    .getSummaryReport(payload.networkId, payload.simulationId)
                    .then((response: AxiosResponse<Blob>) => {
                        if (hasValue(response) && http2XX.test(response.status.toString())) {
                            commit('reportsBlobMutator', new Blob([response.data]));
                            commit('currentReportNameMutator', 'SummaryReport.xlsx');
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