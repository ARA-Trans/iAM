import {Criteria, emptyCriteria} from '@/shared/models/iAM/criteria';
import {parseCriteriaString} from '@/shared/utils/criteria-editor-parsers';
import {clone} from 'ramda';

const state = {
    criteria: {...emptyCriteria} as Criteria
};

const mutations = {
    criteriaMutator(state: any, criteria: Criteria) {
        state.criteria = clone(criteria);
    }
};

const actions = {
    setCriteria({commit}: any, payload: any) {
        commit('criteriaMutator', parseCriteriaString(payload.clause));
    }
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations
};
