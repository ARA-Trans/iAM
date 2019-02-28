import DetailedReportService from '../services/detailed-report.service';

const state = {
    reportBlob: null
};

const mutations = {
    // @ts-ignore
    reportBlobMutator(state, reportBlob) {
        state.reportBlob = reportBlob;
    }
};

const actions = {
    // @ts-ignore
    getDetailedReport({commit}, payload) {
        new DetailedReportService().getDetailedReport(payload.networkId, payload.simulationId)
            .then((reportBlob: Blob) => commit('reportBlobMutator', reportBlob))
            .catch((error: any) => console.log(error));
    },
    // @ts-ignore
    clearReportBlob({commit}) {
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
