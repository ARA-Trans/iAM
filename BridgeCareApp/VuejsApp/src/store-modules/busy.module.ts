const state = {
    isBusy: false
};

const mutations = {
    isBusyMutator(state: any, isBusy: boolean) {
        state.isBusy = isBusy;
    }
};

const actions = {
    setIsBusy({commit}: any, payload: any) {
        commit('isBusyMutator', payload.isBusy);
    }
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations
};
