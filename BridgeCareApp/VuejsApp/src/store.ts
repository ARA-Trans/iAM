import Vue from 'vue'
import Vuex from 'vuex'
import INetwork from '@/models/INetwork'
import Simulation from '@/models/Simulation'

Vue.use(Vuex)

export default new Vuex.Store({
    state: {
        loginFailed: true,
        userName: '',
        networks: [] as INetwork[],
        simulations: [] as Simulation[]
    },
    mutations: {
        login(state, payload) {
            state.loginFailed = payload.status
        },
        userName(state, payload) {
            state.userName = payload.name
        },
        networks(state, payload) {
            state.networks.push(...payload.data)
        },
        simulations(state, payload) {
            state.simulations.push(...payload.data)
        }
    },
    actions: {
        login({ commit }, payload) {
            commit('login', payload)
        },
        userName({ commit }, payload) {
            commit('userName', payload)
        },
        networks({ commit }, payload) {
            commit('networks', payload)
        },
        simulations({ commit }, payload) {
            commit('simulations', payload)
        }
    },
    getters: {
        loginStatus(state) {
            return state.loginFailed
        },
        userName(state) {
            return state.userName
        },
        networks(state) {
            return state.networks as INetwork[]
        },
        simulations(state) {
            return state.simulations as Simulation[]
        }
    }
})