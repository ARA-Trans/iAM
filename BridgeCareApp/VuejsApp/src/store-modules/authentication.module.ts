import AuthenticationService from '../services/authentication.service';
import {AxiosResponse} from 'axios';
import {UserTokens, UserInfo} from '@/shared/models/iAM/authentication';
import {http2XX} from '@/shared/utils/http-utils';
import {getUserName} from '@/shared/utils/get-user-info';

const state = {
    authenticated: false,
    hasRole: false,
    checkedForRole: false,
    username: ''
};

const mutations = {
    authenticatedMutator(state: any, status: boolean) {
        state.authenticated = status;
    },
    hasRoleMutator(state: any, status: boolean) {
        state.hasRole = status;
    },
    checkedForRoleMutator(state: any, status: boolean) {
        state.checkedForRole = status;
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
            dispatch('refreshTokens').then(() => dispatch('getUserInfo'));
            commit('authenticatedMutator', true);
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
                        const userInfo: UserInfo = JSON.parse(response.data) as UserInfo;
                        const username: string = userInfo.sub.split(',')[0].split('=')[1];
                        commit('hasRoleMutator', userInfo.roles !== undefined);
                        commit('checkedForRoleMutator', true);
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
            // ID token is too long to pass as part of the URL, but it will be passed as the parameter
            // of the Authorization header.
            AuthenticationService.revokeToken('', 'Id').then(()=>
                localStorage.removeItem('UserTokens')
            );
            localStorage.removeItem('TokenExpiration');
            AuthenticationService.revokeToken(userTokens.access_token, 'Access');
            AuthenticationService.revokeToken(userTokens.refresh_token, 'Refresh');
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
