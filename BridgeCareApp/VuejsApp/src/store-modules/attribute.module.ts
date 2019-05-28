import {clone} from 'ramda';
import AttributeService from '@/services/attribute.service';
import {AxiosResponse} from 'axios';
import {http2XX, setStatusMessage} from '@/shared/utils/http-utils';
import {hasValue} from '@/shared/utils/has-value-util';

const state = {
    attributes: [] as string[]
};

const mutations = {
    attributesMutator(state: any, attributes: string[]) {
        state.attributes = clone(attributes);
    }
};

const actions = {
    async getAttributes({dispatch, commit}: any) {
        await AttributeService.getAttributes()
            .then((response: AxiosResponse<string[]>) => {
                if (hasValue(response) && http2XX.test(response.status.toString())) {
                    commit('attributesMutator', response.data);
                } else {
                    dispatch('setErrorMessage', {message: `Failed to get attributes${setStatusMessage(response)}`});
                }
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