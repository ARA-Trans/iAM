import Vue from 'vue'
import Vuex from 'vuex'
import axios from 'axios'

import INetwork from '@/models/INetwork'
import Simulation from '@/models/Simulation'

Vue.use(Vuex)

axios.defaults.baseURL = process.env.VUE_APP_URL

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
            state.networks.push(...payload)
        },
        simulations(state, payload) {
            state.simulations.push(...payload)
        }
    },
    actions: {
        login({ commit }, payload) {
            commit('login', payload)
        },
        userName({ commit }, payload) {
            commit('userName', payload)
        },
        fillNetworks({ commit }, payload) {
            commit('networks', payload.data)
        },
        fillSimulations({ commit }, payload) {
            commit('simulations', payload.data)
        },
        async getNetworks({ commit }) {
            await axios
                .get('/api/Networks')
                .then(response => (response.data as Promise<INetwork[]>))
                .then(payload => {
                    commit('networks', payload)
                });
        },
        async getSimulations({ commit }, payload) {
            await axios
                .get(`/api/Simulations/${payload.id}`)
                .then(response => (response.data as Promise<Simulation[]>))
                .then(data => {
                    commit('simulations', data)
                });
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