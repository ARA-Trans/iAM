import AuthenticationService from '../services/authentication.service';
import {AxiosResponse} from 'axios';
import {db} from '@/firebase';

const usersData = ['bridgecareAdministrator', 'testRole'] as Array<string>;

const state = {
    loginFailed: true,
    userName: '',
    userId: '',
    userRoles: [] as Array<string>
};

const mutations = {
    loginMutator(state: any, status: boolean) {
        state.loginFailed = status;
    },
    userNameMutator(state: any, userName: string) {
        state.userName = userName;
    },
    userIdMutator(state: any, userId: string) {
        state.userId = userId;
    },
    userRoleMutator(state: any, role: string) {
        if (!state.userRoles.includes(role)) {
            state.userRoles.push(role);
        }
    }
};

const actions = {
    async authenticateUser({ commit }: any) {
        return await new AuthenticationService().authenticateUser()
            .then((response: AxiosResponse) => {
                if (response.status == 200) {
                    commit('loginMutator', false);
                    commit('userNameMutator', response.data[0]);
                    commit('userIdMutator', response.data[1]);

                    db.ref('roles').once('value', (snapshot: any) => {
                        let data = snapshot.val();
                        for (let key in data) {
                            if (usersData.includes(data[key])) {
                                commit('userRoleMutator', key);
                            }
                        }
                    });
                }
            });
    }
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations
};
