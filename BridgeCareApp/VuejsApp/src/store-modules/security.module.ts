const state = {
    loginFailed: true,
    username: ''
};

const mutations = {
    // @ts-ignore
    loginMutator(state, status) {
        state.loginFailed = status;
    },
    // @ts-ignore
    usernameMutator(state, username) {
        state.username = username;
    }
};

const actions = {
    // @ts-ignore
    setLoginStatus({commit}, payload) {
        commit('loginMutator', payload.status);
    },
    // @ts-ignore
    setUsername({commit}, payload) {
        commit('usernameMutator', payload.username);
    }
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations
};
