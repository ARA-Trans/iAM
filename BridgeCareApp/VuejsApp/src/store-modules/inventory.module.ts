import {Section} from '@/models/section';
import InventoryService from '@/services/inventory.service';

const state = {
    sections: [] as Section[],
};

const mutations = {
    // @ts-ignore
    sectionsMutator(state, sections) {
        state.sections = sections;
    }
};

const actions = {
    // @ts-ignore
    getNetworkInventory({commit}, payload) {
        new InventoryService().getNetworkInventory(payload.network)
            .then((sections: Section[]) =>
                commit('sectionsMutator', sections)
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
