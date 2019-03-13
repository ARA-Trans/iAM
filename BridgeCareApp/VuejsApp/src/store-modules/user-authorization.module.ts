const state = {
    isAdmin: false
};

const mutations = {
    isAdminMutator(state: any, isAdmin: boolean) {
        state.isAdmin = isAdmin;
    }
};

const actions = {
    setIsAdmin({ commit }: any, payload: any) {
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