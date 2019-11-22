import AuthenticationService from '../services/authentication.service';
import {AxiosResponse} from 'axios';
import {UserInformation} from '@/shared/models/iAM/user-information';
import {hasValue} from '@/shared/utils/has-value-util';

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
    }
};

const actions = {
    async authenticateUser({commit}: any) {
        return await AuthenticationService.authenticateUser()
            .then((response: AxiosResponse<UserInformation>) => {
                if (hasValue(response, 'data')) {
                    commit('loginMutator', false);
                    commit('userNameMutator', response.data.name);
                    commit('userIdMutator', response.data.id);
                }
            });
    },

    async getUserTokens({commit}: any, code: string) {
        return await AuthenticationService.getUserTokens(code)
            .then((response: AxiosResponse<any>) => {
                // Don't know what the response will look like yet
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
