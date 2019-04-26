const state = {
    successMessage: '',
    errorMessage: ''
};

const mutations = {
    successMessageMutator(state: any, message: string) {
        state.successMessage = message;
    },
    errorMessageMutator(state: any, message: string) {
        state.errorMessage = message;
    }
};

const actions = {
    setSuccessMessage({commit}: any, payload: any) {
        commit('successMessageMutator', payload.message);
    },
    setErrorMessage({commit}: any, payload: any) {
        commit('errorMessageMutator', payload.message);
    }
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations
};
