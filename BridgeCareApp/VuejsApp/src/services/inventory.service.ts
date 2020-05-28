import {AxiosPromise} from 'axios';
import {axiosInstance} from '@/shared/utils/axios-instance';

export default class InventoryService {
    /**
     * Gets a list of inventory items
     */
    static getInventory(): AxiosPromise {
        return axiosInstance.get('/api/GetInventory');
    }

    /**
     * Gets an inventory item's detail by bms id
     * @param bmsId number
     */
    static getInventoryItemDetailByBMSId(bmsId: string): AxiosPromise {
        return axiosInstance.get('/api/GetInventoryItemDetailByBmsId', {params: {'bmsId': bmsId}});
    }

    /**
     * Gets an inventory item's detail by br key
     * @param brKey number
     */
    static getInventoryItemDetailByBRKey(brKey: string): AxiosPromise {
        return axiosInstance.get('/api/GetInventoryItemDetailByBrKey', {params: {'brKey': brKey}});
    }
}
