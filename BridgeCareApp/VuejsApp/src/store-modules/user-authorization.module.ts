const state = {
    isAdmin: false
};

const mutations = {
    // @ts-ignore
    isAdminMutator(state, isAdmin) {
        state.isAdmin = isAdmin;
    }
};

const actions = {
    // @ts-ignore
    setIsAdmin({ commit }, payload) {
        commit('isAdminMutator', payload.isAdmin);
    }
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations
};