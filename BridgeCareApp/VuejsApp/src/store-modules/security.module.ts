import AuthenticationService from '../services/authentication.service';
import { db } from '@/firebase';

const usersData = ['bridgecareAdministrator', 'testRole'] as Array<string>;

const state = {
    loginFailed: true,
    userName: '',
    userRoles: [] as Array<string>
};

const mutations = {
    loginMutator(state: any, status: boolean) {
        state.loginFailed = status;
    },
    userNameMutator(state: any, userName: string) {
        state.userName = userName;
    },
    userRoleMutator(state: any, role: string) {
        if (!state.userRoles.includes(role)) {
            state.userRoles.push(role);
        }
    }
};

const actions = {
    setLoginStatus({commit}: any, payload: any) {
        commit('loginMutator', payload.status);
    },
    setUsername({commit}: any, payload: any) {
        commit('userNameMutator', payload.userName);
    },
    async getAuthentication({ commit }: any) {
        return await new AuthenticationService().getAuthentication()
            .then((results: any) => {
                if (results.status == '200') {
                    commit('userNameMutator', results.data);
                    commit('loginMutator', false);

                    db.ref('roles').once('value', (snapshot: any) => {
                        let data = snapshot.val();
                        for (let key in data) {
                            if (usersData.includes(data[key])) {
                                commit('userRoleMutator', key);
                            }
                        }
                    });
                    return results;
                }
                else {
                    return results;
                }
            })
            .catch((error: any) => { return error.response });
    }
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations
};
