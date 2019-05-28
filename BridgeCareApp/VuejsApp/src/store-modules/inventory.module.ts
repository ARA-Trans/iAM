import {emptyInventoryItemDetail, InventoryItem, InventoryItemDetail} from '@/shared/models/iAM/inventory';
import InventoryService from '@/services/inventory.service';
import {clone} from 'ramda';
import {AxiosResponse} from 'axios';
import {http2XX, setStatusMessage} from '@/shared/utils/http-utils';
import {hasValue} from '@/shared/utils/has-value-util';

const state = {
    inventoryItems: [] as InventoryItem[],
    inventoryItemDetail: emptyInventoryItemDetail as InventoryItemDetail
};

const mutations = {
    inventoryItemsMutator(state: any, inventoryItems: InventoryItem[]) {
        state.inventoryItems = clone(inventoryItems);
    },
    inventoryItemDetailMutator(state: any, inventoryItemDetail: InventoryItemDetail) {
        state.inventoryItemDetail = clone(inventoryItemDetail);
    }
};

const actions = {
    async getInventory({dispatch, commit}: any) {
        await InventoryService.getInventory()
            .then((response: AxiosResponse<InventoryItem[]>) => {
                if (hasValue(response) && http2XX.test(response.status.toString())) {
                    commit('inventoryItemsMutator', response.data);
                } else {
                    dispatch('setErrorMessage', `Failed to get inventory items${setStatusMessage(response)}`);
                }
            });
    },
    async getInventoryItemDetailByBMSId({dispatch, commit}: any, payload: any) {
        await InventoryService.getInventoryItemDetailByBMSId(payload.bmsId)
            .then((response: AxiosResponse<InventoryItemDetail>) => {
                if (hasValue(response) && http2XX.test(response.status.toString())) {
                    commit('inventoryItemDetailMutator', response.data);
                } else {
                    dispatch('setErrorMessage', `Failed to get inventory item detail${setStatusMessage(response)}`);
                }
            });
    },
    async getInventoryItemDetailByBRKey({dispatch, commit}: any, payload: any) {
        await InventoryService.getInventoryItemDetailByBRKey(payload.brKey)
            .then((response: AxiosResponse<InventoryItemDetail>) => {
                if (hasValue(response) && http2XX.test(response.status.toString())) {
                    commit('inventoryItemDetailMutator', response.data);
                } else {
                    dispatch('setErrorMessage', `Failed to get inventory item detail${setStatusMessage(response)}`);
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
