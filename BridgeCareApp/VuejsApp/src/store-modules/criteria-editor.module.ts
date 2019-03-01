import {Criteria, CriteriaEditorAttribute, emptyCriteria} from '@/models/criteria';
import CriteriaEditorService from '@/services/criteria-editor.service';
import {parseCriteriaString} from '@/shared/utils/criteria-editor-parsers';

const state = {
    criteriaEditorAttributes: [] as CriteriaEditorAttribute[],
    criteria: emptyCriteria as Criteria
};

const mutations = {
    // @ts-ignore
    criteriaEditorAttributesMutator(state, criteriaEditorAttributes) {
        state.criteriaEditorAttributes = criteriaEditorAttributes;
    },
    // @ts-ignore
    criteriaMutator(state, criteria) {
        state.criteria = criteria;
    }
};

const actions = {
    // @ts-ignore
    setCriteriaEditorAttributes({commit}, payload) {
        new CriteriaEditorService().getCriteriaEditorAttributes()
            .then((criteriaEditorAttributes: CriteriaEditorAttribute[]) =>
                commit('criteriaEditorAttributesMutator', criteriaEditorAttributes)
            )
            .catch((error: any) => console.log(error));
    },
    // @ts-ignore
    setCriteria({commit}, payload) {
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
