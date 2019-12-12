import AuthenticationService from '../services/authentication.service';
import {AxiosResponse} from 'axios';
import {UserTokens} from '@/shared/models/iAM/authentication';
import {http2XX} from '@/shared/utils/http-utils';
import { getUserName } from '@/shared/utils/get-user-info';

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
                    const expiration: Date = new Date(Date.now() + 30 * 60 * 1000); // 30 minutes, in milliseconds
                    localStorage.setItem('TokenExpiration', expiration.getTime().toString());
                    commit('authenticatedMutator', true);
                }
            });
    },

    async checkBrowserTokens({commit, dispatch}: any, code: string) {
        const storedTokenExpiration: number = Number(localStorage.getItem('TokenExpiration') as string);
        if (isNaN(storedTokenExpiration)) {
            return;
        }
        if (storedTokenExpiration > Date.now()) {
            if (state.authenticated) {
                return;
            }
            commit('authenticatedMutator', true);
            dispatch('refreshTokens');
            dispatch('getUserInfo');
        } else if (state.authenticated) {
            dispatch('logOut');
        }
    },

    async refreshTokens({commit, dispatch}: any) {
        if (!localStorage.getItem('UserTokens')) {
            dispatch('logOut');
        } else {
            const userTokens: UserTokens = JSON.parse(localStorage.getItem('UserTokens') as string) as UserTokens;
            await AuthenticationService.refreshTokens(userTokens.refresh_token)
                .then((response: AxiosResponse<string>) => {
                    if (http2XX.test(response.status.toString())) {
                        localStorage.setItem('UserTokens', JSON.stringify({
                            ...userTokens,
                            ...JSON.parse(response.data)
                        }));
                        const expiration: Date = new Date(Date.now() + 30 * 60 * 1000); // 30 minutes, in milliseconds
                        localStorage.setItem('TokenExpiration', expiration.getTime().toString());
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
                        const username: string = getUserName();
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
            localStorage.removeItem('TokenExpiration');
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
