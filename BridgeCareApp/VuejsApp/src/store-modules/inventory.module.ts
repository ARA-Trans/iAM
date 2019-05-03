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
    async getInventory({ commit }: any) {
        await new InventoryService().getInventory()
            .then((inventoryItems: InventoryItem[]) =>
                commit('inventoryItemsMutator', inventoryItems)
            )
    },
    async getInventoryItemDetailByBMSId({ commit }: any, payload: any) {
        await new InventoryService().getInventoryItemDetailByBMSId(payload.bmsId)
            .then((inventoryItemDetail: InventoryItemDetail) =>
                commit('inventoryItemDetailMutator', inventoryItemDetail)
            )
    },
    async getInventoryItemDetailByBRKey({ commit }: any, payload: any) {
        await new InventoryService().getInventoryItemDetailByBRKey(payload.brKey)
            .then((inventoryItemDetail: InventoryItemDetail) =>
                commit('inventoryItemDetailMutator', inventoryItemDetail)
            )
    }
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations
};
