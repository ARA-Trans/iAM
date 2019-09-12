import {clone} from 'ramda';
import AttributeService from '@/services/attribute.service';
import {AxiosResponse} from 'axios';
import {Attribute} from '@/shared/models/iAM/attribute';

const state = {
    attributes: [] as Attribute[],
    stringAttributes: [] as Attribute[],
    numericAttributes: [] as Attribute[]
};

const mutations = {
    attributesMutator(state: any, attributes: string[]) {
        state.attributes = clone(attributes);
    },
    stringAttributesMutator(state: any, stringAttributes: string[]) {
        state.stringAttributes = clone(stringAttributes);
    },
    numericAttributesMutator(state: any, numericAttributes: string[]) {
        state.numericAttributes = clone(numericAttributes);
    }
};

const actions = {
    async getAttributes({dispatch, commit}: any) {
        await AttributeService.getAttributes()
            .then((response: AxiosResponse<Attribute[]>) => {
                commit('attributesMutator', response.data);
                commit('stringAttributesMutator', response.data
                    .filter((attribute: Attribute) => attribute.type === 'STRING'));
                commit('numericAttributesMutator', response.data
                    .filter((attribute: Attribute) => attribute.type === 'NUMBER'));
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