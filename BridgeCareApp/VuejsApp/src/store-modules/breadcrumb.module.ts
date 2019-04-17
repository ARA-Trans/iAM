import { clone } from 'ramda';

const state = {
    navigation: []
};

const mutations = {
    isNavigationMutator(state: any, navigation: any) {
        state.navigation = clone(navigation);
    }
};

const actions = {
    setNavigation({ commit }: any, payload: any) {
        commit('isNavigationMutator', payload);
    }
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations
};
