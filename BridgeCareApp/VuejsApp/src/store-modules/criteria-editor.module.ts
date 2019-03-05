import {Criteria, CriteriaEditorAttribute, emptyCriteria} from '@/models/iAM/criteria';
import CriteriaEditorService from '@/services/criteria-editor.service';
import {parseCriteriaString} from '@/shared/utils/criteria-editor-parsers';

const state = {
    criteriaEditorAttributes: [] as CriteriaEditorAttribute[],
    criteria: emptyCriteria as Criteria
};

const mutations = {
    criteriaEditorAttributesMutator(state: any, criteriaEditorAttributes: CriteriaEditorAttribute[]) {
        state.criteriaEditorAttributes = criteriaEditorAttributes;
    },
    criteriaMutator(state: any, criteria: Criteria) {
        state.criteria = criteria;
    }
};

const actions = {
    getCriteriaEditorAttributes({commit}: any, payload: any) {
        new CriteriaEditorService().getCriteriaEditorAttributes()
            .then((criteriaEditorAttributes: CriteriaEditorAttribute[]) =>
                commit('criteriaEditorAttributesMutator', criteriaEditorAttributes)
            )
            .catch((error: any) => console.log(error));
    },
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
