import {Section} from '@/models/iAM/section';
import InventoryService from '@/services/inventory.service';

const state = {
    sections: [] as Section[],
};

const mutations = {
    sectionsMutator(state: any, sections: Section) {
        state.sections = sections;
    }
};

const actions = {
    getNetworkInventory({commit}: any, payload: any) {
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
