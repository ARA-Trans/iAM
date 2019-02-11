import Vue from 'vue'
import Vuex from 'vuex'

Vue.use(Vuex)

export default new Vuex.Store({
    state: {
        loginFailed: true,
        userName: ''
    },
    mutations: {
        login(state, payload) {
            state.loginFailed = payload.status
        },
        userName(state, payload) {
            state.userName = payload.name
        }
    },
    actions: {
        login({ commit }, payload) {
            commit('login', payload)
        },
        userName({ commit }, payload) {
            commit('userName', payload)
        }
    },
    getters: {
        loginStatus(state) {
            return state.loginFailed
        },
        userName(state) {
            return state.userName
        }
    }
})