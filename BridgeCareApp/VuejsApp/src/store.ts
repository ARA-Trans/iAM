import Vue from 'vue'
import Vuex from 'vuex'
import axios from 'axios'

import INetwork from '@/models/INetwork'
import Simulation from '@/models/Simulation'
import {IScenario} from '@/models/scenario';
import * as R from 'ramda';
import * as moment from 'moment';
import {attributes, sharedScenarios, userScenarios} from '@/shared/utils/mock-data';
import {Criteria, CriteriaAttribute, emptyCriteria} from '@/models/criteria';
import {parseCriteriaString} from '@/shared/utils/criteria-editor-parsers';

Vue.use(Vuex);

axios.defaults.baseURL = process.env.VUE_APP_URL;

export default new Vuex.Store({
    state: {
        loginFailed: true,
        userName: '',
        networks: [] as INetwork[],
        simulations: [] as Simulation[],
        scenarios: [] as IScenario[],
        criteriaAttributes: [] as CriteriaAttribute[],
        criteria: emptyCriteria as Criteria
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
        },
        criteriaAttributes(state, payload) {
            state.criteriaAttributes = payload;
        },
        criteria(state, payload) {
            state.criteria = parseCriteriaString(payload.clause);
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
        },
        async getCriteriaAttributes({commit}, payload) {
            // TODO: integrate web service to get criteria editor attributes, for now just create some mock data
            const criteriaAttributes = attributes.map((a: string) => {
                return {
                    name: a,
                    values: ['1', '2', '3', '4', '5', '6', '7', '8', '9', '10']
                };
            });
            commit('criteriaAttributes', criteriaAttributes);
            /*axios
                .get('/api/GetCriteriaEditorAttributes')
                .then(response => (response.data as Promise<CriteriaAttribute[]>))
                .then(data => {
                    commit('criteriaAttributes', data)
                });*/
        },
        setCriteria({commit}, payload) {
            commit('criteria', payload);
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
        },
        criteriaAttributes(state) {
            return state.criteriaAttributes
        },
        criteria(state) {
            return state.criteria as Criteria
        }
    }
})