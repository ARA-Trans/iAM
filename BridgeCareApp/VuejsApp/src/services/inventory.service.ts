import {AxiosPromise} from 'axios';
import {InventoryItem, InventoryItemDetail} from '@/shared/models/iAM/inventory';
import {axiosInstance} from '@/shared/utils/axios-instance';

export default class InventoryService {
    /**
     * Gets a list of inventory items
     */
    static getInventory(): AxiosPromise<InventoryItem[]> {
        return axiosInstance.get<InventoryItem[]>('/api/InventorySelectionModels');
    }

    /**
     * Gets an inventory item's detail by bms id
     * @param bmsId number
     */
    static getInventoryItemDetailByBMSId(bmsId: number): AxiosPromise<InventoryItemDetail> {
        return axiosInstance
            .get<InventoryItemDetail>('/api/InventoryItemDetailByBMSId', {params: {'bmsId': bmsId}});
    }

    /**
     * Gets an inventory item's detail by br key
     * @param brKey number
     */
    static getInventoryItemDetailByBRKey(brKey: number): AxiosPromise<InventoryItemDetail> {
        return axiosInstance
            .get<InventoryItemDetail>('/api/InventoryItemDetailByBRKey', {params: {'brKey': brKey}});
    }
}
