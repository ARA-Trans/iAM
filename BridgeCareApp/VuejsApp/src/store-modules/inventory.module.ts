import {emptyInventoryItemDetail, InventoryItem, InventoryItemDetail} from '@/shared/models/iAM/inventory';
import InventoryService from '@/services/inventory.service';
import {append, clone, contains} from 'ramda';
import {AxiosResponse} from 'axios';
import {hasValue} from '@/shared/utils/has-value-util';

const state = {
    inventoryItems: [] as InventoryItem[],
    inventoryItemDetail: emptyInventoryItemDetail as InventoryItemDetail,
    lastFiveBmsIdSearches: [] as string[],
    lastFiveBrKeySearches: [] as number[]
};

const mutations = {
    inventoryItemsMutator(state: any, inventoryItems: InventoryItem[]) {
        state.inventoryItems = clone(inventoryItems);
    },
    inventoryItemDetailMutator(state: any, inventoryItemDetail: InventoryItemDetail) {
        state.inventoryItemDetail = clone(inventoryItemDetail);
    },
    lastFiveBmsIdSearchesMutator(state: any, searchString: string) {
        if (!contains(searchString, state.lastFiveBmsIdSearches)) {
            if (state.lastFiveBmsIdSearches.length === 5) {
                const filteredSearches = state.lastFiveBmsIdSearches
                    .filter((value: string) => value !== state.lastFiveBmsIdSearches[0]);
                state.lastFiveBmsIdSearches = append(searchString, filteredSearches);
            } else {
                state.lastFiveBmsIdSearches = append(searchString, state.lastFiveBmsIdSearches);
            }
        }
    },
    lastFiveBrKeySearchesMutator(state: any, searchNumber: number) {
        if (!contains(searchNumber, state.lastFiveBrKeySearches)) {
            if (state.lastFiveBrKeySearches.length === 5) {
                const filteredSearches = state.lastFiveBrKeySearches
                    .filter((value: number) => value !== state.lastFiveBrKeySearches[0]);
                state.lastFiveBrKeySearches = append(searchNumber, filteredSearches);
            } else {
                state.lastFiveBrKeySearches = append(searchNumber, state.lastFiveBrKeySearches);
            }
        }
    }
};

const actions = {
    clearInventoryItemDetail({commit}: any) {
        commit('inventoryItemDetailMutator', emptyInventoryItemDetail);
    },
    appendBmsIdSearchString({commit}: any, payload: any) {
        commit('lastFiveBmsIdSearchesMutator', payload.bmsId);
    },
    appendBrKeySearchNumber({commit}: any, payload: any) {
        commit('lastFiveBrKeySearchesMutator', payload.brKey);
    },
    async getInventory({commit}: any) {
        await InventoryService.getInventory()
            .then((response: AxiosResponse<InventoryItem[]>) => {
                if (hasValue(response, 'data')) {
                    commit('inventoryItemsMutator', response.data);
                }
            });
    },
    async getInventoryItemDetailByBMSId({commit}: any, payload: any) {
        await InventoryService.getInventoryItemDetailByBMSId(payload.bmsId)
            .then((response: AxiosResponse<InventoryItemDetail>) => {
                if (hasValue(response, 'data')) {
                    commit('inventoryItemDetailMutator', response.data);
                }
            });
    },
    async getInventoryItemDetailByBRKey({commit}: any, payload: any) {
        await InventoryService.getInventoryItemDetailByBRKey(payload.brKey)
            .then((response: AxiosResponse<InventoryItemDetail>) => {
                if (hasValue(response, 'data')) {
                    commit('inventoryItemDetailMutator', response.data);
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
