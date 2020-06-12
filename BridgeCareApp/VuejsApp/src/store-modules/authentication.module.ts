import AuthenticationService from '../services/authentication.service';
import {AxiosResponse} from 'axios';
import {UserInfo, UserTokens} from '@/shared/models/iAM/authentication';
import {http2XX} from '@/shared/utils/http-utils';
import {checkLDAP, parseLDAP, regexCheckLDAP} from '@/shared/utils/parse-ldap';

const state = {
    authenticated: false,
    hasRole: false,
    checkedForRole: false,
    isAdmin: false,
    isCWOPA: false,
    username: '',
    refreshing: false
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
    isAdminMutator(state: any, status: boolean) {
        state.isAdmin = status;
    },
    isCWOPAMutator(state: any, status: boolean) {
        state.isCWOPA = status;
    },
    usernameMutator(state: any, username: string) {
        state.username = username;
    },
    refreshingMutator(state: any, refreshing: boolean) {
        state.refreshing = refreshing;
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
            
            // Only refresh the token when it has a minute or less before expiration
            if (storedTokenExpiration - Date.now() < 60000) {
                dispatch('refreshTokens').then(() => dispatch('getUserInfo'));
            } else {
                dispatch('getUserInfo');
            }
            
            commit('authenticatedMutator', true);
        } else if (state.authenticated) {
            dispatch('logOut');
        }
    },

    async refreshTokens({commit, dispatch}: any) {
        if (!localStorage.getItem('UserTokens')) {
            dispatch('logOut');
        } else {
            commit('refreshingMutator', true);
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
            commit('refreshingMutator', false);
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
                        const username: string = parseLDAP(userInfo.sub)[0];
                        commit('hasRoleMutator', regexCheckLDAP(userInfo.roles, /PD-BAMS(-(Administrator|CWOPA|DBEngineer)|USERS)/));
                        if (state.hasRole) {
                            commit('isAdminMutator', checkLDAP(userInfo.roles, 'PD-BAMS-Administrator'));
                            commit('isCWOPAMutator', checkLDAP(userInfo.roles, 'PD-BAMS-CWOPA'));
                        }
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
            localStorage.removeItem('UserTokens');
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
