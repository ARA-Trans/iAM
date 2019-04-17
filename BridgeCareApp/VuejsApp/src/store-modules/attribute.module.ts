import {clone} from 'ramda';
import AttributeService from '@/services/attribute.service';

const state = {
    attributes: [] as string[]
};

const mutations = {
    attributesMutator(state: any, attributes: string[]) {
        state.attributes = clone(attributes);
    }
};

const actions = {
    async getAttributes({commit}: any) {
        await new AttributeService().getAttributes()
            .then((attributes: string[]) =>
                commit('attributesMutator', attributes)
            )
            .catch((error: any) => console.log(error));
    }
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations
};