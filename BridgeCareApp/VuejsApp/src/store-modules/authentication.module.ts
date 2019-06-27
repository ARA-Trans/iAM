import AuthenticationService from '../services/authentication.service';
import {AxiosResponse} from 'axios';
import {db} from '@/firebase';
import {UserInformation} from '@/shared/models/iAM/user-information';

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
    async authenticateUser({dispatch, commit }: any) {
        return await AuthenticationService.authenticateUser()
            .then((response: AxiosResponse<UserInformation>) => {
                commit('loginMutator', false);
                commit('userNameMutator', response.data.name);
                commit('userIdMutator', response.data.id);

                db.ref('roles').once('value', (snapshot: any) => {
                    let data = snapshot.val();
                    for (let key in data) {
                        if (usersData.includes(data[key])) {
                            commit('userRoleMutator', key);
                        }
                    }
                });
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
