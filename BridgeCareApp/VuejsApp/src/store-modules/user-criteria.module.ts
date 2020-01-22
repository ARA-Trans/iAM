import {UserCriteria} from '@/shared/models/iAM/user-criteria';
import {propEq, clone, reject, insert} from 'ramda';
import UserCriteriaService from '@/services/user-criteria.service';
import {AxiosResponse} from 'axios';
import {hasValue} from '@/shared/utils/has-value-util';

const state = {
    allUserCriteria: [] as UserCriteria[]
};

const mutations = {
    allUserCriteriaMutator(state: any, allUserCriteria: UserCriteria[]) {
        state.allUserCriteria = clone(allUserCriteria);
    },
    userCriteriaMutator(state: any, userCriteria: UserCriteria) {
        const allUserCriteria = reject(propEq('username', userCriteria.username), state.allUserCriteria);
        state.allUserCriteria = insert(0, userCriteria, allUserCriteria);
    }
};

const actions = {
    async getUserCriteria({dispatch}: any) {
        const message = 'You do not have access to any bridge data. \
        Please contact an administrator to gain access to the data you need.';
        await UserCriteriaService.getOwnUserCriteria()
            .then((response: AxiosResponse<UserCriteria>) => {
                if (hasValue(response, 'data')) {
                    if (!response.data.hasAccess) {
                        dispatch('setInfoMessage', {message});
                    }
                }
            });
    },
    async getAllUserCriteria({commit}: any) {
        await UserCriteriaService.getAllUserCriteria()
            .then((response: AxiosResponse<UserCriteria[]>) => {
                if (hasValue(response, 'data')) {
                    const allUserCriteria: UserCriteria[] = response.data;
                    commit('allUserCriteriaMutator', allUserCriteria);
                }
            });
    },
    async setUserCriteria({commit}: any, payload: any) {
        await UserCriteriaService.setUserCriteria(payload.userCriteria)
            .then((response: AxiosResponse<UserCriteria>) => {
                commit('userCriteriaMutator', payload.userCriteria);
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
