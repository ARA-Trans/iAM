import {emptyInventoryItemDetail, InventoryItem, InventoryItemDetail} from '@/shared/models/iAM/inventory';
import InventoryService from '@/services/inventory.service';
import * as R from 'ramda';

const state = {
    inventoryItems: [] as InventoryItem[],
    inventoryItemDetail: emptyInventoryItemDetail as InventoryItemDetail
};

const mutations = {
    inventoryItemsMutator(state: any, inventoryItems: InventoryItem[]) {
        state.inventoryItems = R.clone(inventoryItems);
    },
    inventoryItemDetailMutator(state: any, inventoryItemDetail: InventoryItemDetail) {
        state.inventoryItemDetail = R.merge(state.inventoryItemDetail, inventoryItemDetail);
    }
};

const actions = {
    async getInventory({commit}: any, payload: any) {
        await new InventoryService().getInventory(payload.network)
            .then((inventoryItems: InventoryItem[]) =>
                commit('inventoryItemsMutator', inventoryItems)
            )
            .catch((error: any) => console.log(error));
    },
    async getInventoryItemDetail({commit}: any, payload: any) {
        await new InventoryService().getInventoryItemDetail(payload.inventoryItem)
            .then((inventoryItemDetail: InventoryItemDetail) =>
                commit('inventoryItemDetailMutator', inventoryItemDetail)
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
