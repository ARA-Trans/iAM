const state = {
    isBusy: false
};

const mutations = {
    // @ts-ignore
    isBusyMutator(state, isBusy) {
        state.isBusy = isBusy;
    }
};

const actions = {
    // @ts-ignore
    setIsBusy({commit}, payload) {
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
