const state = {
    loginFailed: true,
    userName: ''
};

const mutations = {
    // @ts-ignore
    loginMutator(state, status) {
        state.loginFailed = status;
    },
    // @ts-ignore
    userNameMutator(state, userName) {
        state.userName = userName;
    }
};

const actions = {
    // @ts-ignore
    setLoginStatus({commit}, payload) {
        commit('loginMutator', payload.status);
    },
    // @ts-ignore
    setUsername({commit}, payload) {
        commit('userNameMutator', payload.userName);
    }
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations
};
