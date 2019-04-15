import DetailedReportService from '@/services/detailed-report.service';
import SummaryReportService from '@/services/summary-report.service';

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
    async getReports({ commit }: any, payload: any) {
        switch (payload.reportName) {
            case 'Detailed report': {
                 await new DetailedReportService().getDetailedReport(payload.networkId, payload.simulationId)
                     .then((reportsBlob: Blob) => {
                        commit('reportsBlobMutator', reportsBlob);
                        commit('currentReportNameMutator', 'Detailed report.xlsx')
                    })                    
                    .catch((error: any) => console.log(error));
                break;
            }
            case 'Summary report': {
                await new SummaryReportService().getSummaryReport(payload.networkId, payload.simulationId, payload.networkName, payload.simulationName)
                    .then((reportsBlob: Blob) => {
                        commit('reportsBlobMutator', reportsBlob),
                        commit('currentReportNameMutator', 'Summary report.xlsx')
                        })
                    .catch((error: any) => console.log(error));
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