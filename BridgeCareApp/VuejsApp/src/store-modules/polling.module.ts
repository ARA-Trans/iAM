import PollingService from '@/services/polling.service';
import {AxiosResponse} from 'axios';
import {hasValue} from '@/shared/utils/has-value-util';

const state = {
    sessionId: ''
};

const mutations = {
    sessionIdMutator(state: any, sessionId: string) {
        state.sessionId = sessionId;
    }
};

const actions = {
    async pollEvents({dispatch}: any) {
        if (state.sessionId === '') {
            return;
        }
        await PollingService.pollEvents(state.sessionId)
            .then((response: AxiosResponse<any>) => {
                if (hasValue(response, 'data')) {
                    response.data.map((event: any) => {
                        if (event.eventName === 'criteriaLibrary') {
                            console.log('here it is');
                        }
                        dispatch(`socket_${event.eventName}`, event.payload);
                    });
                }
            });
    },

    /**
     * Creates an identifier that the node server will use to determine
     * if this browser session has already been sent an event.
     */
    async generatePollingSessionId({commit}: any) {
        commit('sessionIdMutator', `${Date.now()}${Math.random()}`);
    }
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations
};
