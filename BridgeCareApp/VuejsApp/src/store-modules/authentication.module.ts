import AuthenticationService from '../services/authentication.service';
import {AxiosResponse} from 'axios';
import {UserInfo, UserTokens} from '@/shared/models/iAM/authentication';
import {http2XX} from '@/shared/utils/http-utils';

const state = {
    authenticated: false,
    username: ''
};

const mutations = {
    authenticatedMutator(state: any, status: boolean) {
        state.authenticated = status;
    },
    usernameMutator(state: any, username: string) {
        state.username = username;
    }
};

const actions = {
    async getUserTokens({commit}: any, code: string) {
        await AuthenticationService.getUserTokens(code)
            .then((response: AxiosResponse<string>) => {
                if (http2XX.test(response.status.toString())) {
                    localStorage.setItem('UserTokens', response.data);
                    commit('authenticatedMutator', true);
                }
            });
    },

    async refreshAccessToken({commit, dispatch}: any) {
        if (!localStorage.getItem('UserTokens')) {
            dispatch('logOut');
        } else {
            const userTokens: UserTokens = JSON.parse(localStorage.getItem('UserTokens') as string) as UserTokens;
            await AuthenticationService.refreshAccessToken(userTokens.refresh_token)
                .then((response: AxiosResponse<string>) => {
                    if (http2XX.test(response.status.toString())) {
                        localStorage.setItem('UserTokens', JSON.stringify({
                            ...userTokens,
                            ...JSON.parse(response.data)
                        }));
                    }
                });
        }
    },

    async getUserInfo({commit, dispatch}: any) {
        if (!localStorage.getItem('UserTokens')) {
            dispatch('logOut');
        } else {
            const userTokens: UserTokens = JSON.parse(localStorage.getItem('UserTokens') as string) as UserTokens;
            await AuthenticationService.getUserInfo(userTokens.access_token)
                .then((response: AxiosResponse<string>) => {
                    if (http2XX.test(response.status.toString())) {
                        localStorage.setItem('UserInfo', response.data);
                        const userInfo: UserInfo = JSON.parse(response.data) as UserInfo;
                        const username: string = userInfo.sub.split(',')[0].split('=')[1];
                        commit('usernameMutator', username);
                    } else {
                        dispatch('logOut');
                    }
                });
        }
    },

    async logOut({commit}: any) {
        if (!localStorage.getItem('UserTokens')) {
            commit('usernameMutator', '');
            commit('authenticatedMutator', false);
        } else {
            localStorage.removeItem('UserInfo');
            const userTokens: UserTokens = JSON.parse(localStorage.getItem('UserTokens') as string) as UserTokens;
            localStorage.removeItem('UserTokens');
            AuthenticationService.revokeToken(userTokens.access_token);
            AuthenticationService.revokeToken(userTokens.refresh_token);
            commit('usernameMutator', '');
            commit('authenticatedMutator', false);
        }
    }
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations
};
