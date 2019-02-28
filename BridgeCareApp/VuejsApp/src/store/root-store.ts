import Vue from 'vue'
import Vuex from 'vuex'
import axios from 'axios'

import INetwork from '../models/INetwork'
import Simulation from '../models/Simulation'
import {IScenario, sharedScenarios, userScenarios} from '../models/scenario';
import * as R from 'ramda';
import * as moment from 'moment';
import {mockSectionDetail, mockSections, Section, SectionDetail} from '../models/section';
import {hasValue} from '../shared/utils/has-value';

Vue.use(Vuex);

axios.defaults.baseURL = process.env.VUE_APP_URL;

export default new Vuex.Store({
    state: {
        isBusy: false,
        loginFailed: true,
        userName: '',
        networks: [] as INetwork[],
        simulations: [] as Simulation[],
        scenarios: [] as IScenario[],
        sections: [] as Section[],
        sectionDetail: {} as SectionDetail
    },
    mutations: {
        isBusy(state, payload) {
            state.isBusy = payload
        },
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
        sections(state, payload) {
            state.sections = payload;
        },
        sectionDetail(state, payload) {
            state.sectionDetail = payload;
        }
    },
    actions: {
        setIsBusy({commit}, payload) {
            commit('isBusy', payload.isBusy)
        },
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
            commit('isBusy', true);
            await axios
                .get('/api/Networks')
                .then(response => (response.data as Promise<INetwork[]>))
                .then(payload => {
                    commit('isBusy', false);
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
        async getUserScenarios({commit}, payload) {
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
            await commit('scenarios', scenarios);
            // TODO: integrate web service to get user scenarios, for now just create some mock data
            /*await axios
                .get(`/api/GetUserScenarios?userId=${payload.userId}`)
                .then(response => (response.data as Promise<IScenario[]>))
                .then(data => {
                    commit('scenarios', data)
                });*/
        },
        async getNetworkInventory({commit}, payload) {
            await commit('sections', mockSections);
            // TODO: integrate web service to get sections for the inventory view
            /*await axios
                .get(`/api/GetNetworkInventory?networkId=${payload.networkId}`)
                .then(response => (response.data as Promise<Section[]>))
                .then(data => {
                    commit('sections', data)
                });*/
        },
        async getInventoryItemDetail({commit}, payload) {
            if (hasValue(payload.section)) {
                await commit('sectionDetail', mockSectionDetail);
            } else {
                await commit('sectionDetail', null);
            }
            // TODO: integreate web service to get section details for the inventory view
            /*await axios
                .get(`/api/GetInventoryItemDetails?sectionId=${payload.sectionId}&networkId=${payload.networkId}`)
                .then(response => (response.data as Promise<SectionDetail>))
                .then(data => {
                    commit('sectionDetail', data)
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
        },
        sections(state) {
            return state.sections
        },
        sectionDetail(state) {
            return state.sectionDetail
        }
    }
})