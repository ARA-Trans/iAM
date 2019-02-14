import Vue from 'vue'
import Vuex from 'vuex'
import axios from 'axios'

import INetwork from '@/models/INetwork'
import Simulation from '@/models/Simulation'
import {IScenario, sharedScenarios, userScenarios} from '@/models/scenario';
import * as R from 'ramda';
import * as moment from 'moment';

Vue.use(Vuex);

axios.defaults.baseURL = process.env.VUE_APP_URL;

export default new Vuex.Store({
    state: {
        loginFailed: true,
        userName: '',
        networks: [] as INetwork[],
        simulations: [] as Simulation[],
        scenarios: [] as IScenario[]
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
        },
        scenarios(state, payload) {
            if (R.isEmpty(state.scenarios)) {
                state.scenarios.push(...payload);
            } else {
                //@ts-ignore
                // groupBy function that will group by scenarioId
                const groupById = R.groupBy(R.prop('scenarioId'));
                // group all scenarios using the groupById function
                const grouped = groupById([...state.scenarios, ...payload]);
                // sort the scenario groups and get only the latest scenario from each sorted group
                const latest = R.keys(grouped).map(id => {
                    const sortByLastModifiedDate = R.sortBy(R.prop('lastModifiedDate'));
                    const sortedGroup = sortByLastModifiedDate(grouped[id]);
                    return R.last(sortedGroup);
                });
                // update state with only the latest scenarios
                state.scenarios = latest as IScenario[];
            }
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
        getNetworks({ commit }) {
            axios
                .get('/api/Networks')
                .then(response => (response.data as Promise<INetwork[]>))
                .then(payload => {
                    commit('networks', payload)
                });
        },
        getSimulations({ commit }, payload) {
            axios
                .get(`/api/Simulations/${payload.id}`)
                .then(response => (response.data as Promise<Simulation[]>))
                .then(data => {
                    commit('simulations', data)
                });
        },
        getUserScenarios({commit}, payload) {
            // TODO: integrate web service to get user scenarios, for now just create some mock data
            //@ts-ignore
            const dateMinus2Days = new Date(moment().subtract(2, 'days').format('YYYY-MM-DDTHH:mm:ss.SSS'));
            const scenarios = [
                ...userScenarios.map((s: IScenario) => {
                    return {
                        ...s,
                        createdDate: dateMinus2Days
                    }
                }),
                ...sharedScenarios.map((s: IScenario) => {
                    return {
                        ...s,
                        createdDate: dateMinus2Days
                    }
                })
            ];
            commit('scenarios', scenarios);
            /*axios
                .get(`/api/GetUserScenarios?userId=${payload.userId}`)
                .then(response => (response.data as Promise<IScenario[]>))
                .then(data => {
                    commit('scenarios', data)
                });*/
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
        },
        scenarios(state) {
            return state.scenarios
        }
    }
})