import {AxiosPromise} from 'axios';
import {axiosInstance} from '@/shared/utils/axios-instance';

export default class InventoryService {
    /**
     * Gets a list of inventory items
     */
    static getInventory(): AxiosPromise {
        return axiosInstance.get('/api/InventorySelectionModels');
    }

    /**
     * Gets an inventory item's detail by bms id
     * @param bmsId number
     */
    static getInventoryItemDetailByBMSId(bmsId: number): AxiosPromise {
        return axiosInstance.get('/api/InventoryItemDetailByBMSId', {params: {'bmsId': bmsId}});
    }

    /**
     * Gets an inventory item's detail by br key
     * @param brKey number
     */
    static getInventoryItemDetailByBRKey(brKey: number): AxiosPromise {
        return axiosInstance.get('/api/InventoryItemDetailByBRKey', {params: {'brKey': brKey}});
    }
}
