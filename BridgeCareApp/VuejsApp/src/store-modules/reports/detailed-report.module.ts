import DetailedReportService from '@/services/detailed-report.service';

const state = {
    reportBlob: null
};

const mutations = {
    reportBlobMutator(state: any, reportBlob: Blob) {
        state.reportBlob = reportBlob;
    }
};

const actions = {
    async getDetailedReport({commit}: any, payload: any) {
        await new DetailedReportService().getDetailedReport(payload.networkId, payload.simulationId)
            .then((reportBlob: Blob) => commit('reportBlobMutator', reportBlob))
            .catch((error: any) => console.log(error));
    },
    clearReportBlob({commit}: any) {
        commit('reportBlobMutator', null);
    }
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations
};
