const state = {
    hasUnsavedChanges: false
};

const mutations = {
    hasUnsavedChangesMutator(state: any, value: boolean) {
        state.hasUnsavedChanges = value;
    }
};

const actions = {
    setHasUnsavedChanges({commit}: any, payload: any) {
        commit('hasUnsavedChangesMutator', payload.value);
    }
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations
};
