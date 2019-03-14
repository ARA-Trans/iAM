const state = {
    loginFailed: true,
    userName: ''
};

const mutations = {
    loginMutator(state: any, status: boolean) {
        state.loginFailed = status;
    },
    userNameMutator(state: any, userName: string) {
        state.userName = userName;
    }
};

const actions = {
    setLoginStatus({commit}: any, payload: any) {
        commit('loginMutator', payload.status);
    },
    setUsername({commit}: any, payload: any) {
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
