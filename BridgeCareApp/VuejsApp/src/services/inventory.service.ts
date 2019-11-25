import {AxiosPromise} from 'axios';
import {axiosInstance} from '@/shared/utils/axios-instance';
import {getAuthHeader} from '@/shared/utils/authentication-header';

export default class InventoryService {
    /**
     * Gets a list of inventory items
     */
    static getInventory(): AxiosPromise {
        return axiosInstance.get('/api/GetInventory', {headers: getAuthHeader()});
    }

    /**
     * Gets an inventory item's detail by bms id
     * @param bmsId number
     */
    static getInventoryItemDetailByBMSId(bmsId: number): AxiosPromise {
        return axiosInstance.get('/api/GetInventoryItemDetailByBmsId', {params: {'bmsId': bmsId}, headers: getAuthHeader()});
    }

    /**
     * Gets an inventory item's detail by br key
     * @param brKey number
     */
    static getInventoryItemDetailByBRKey(brKey: number): AxiosPromise {
        return axiosInstance.get('/api/GetInventoryItemDetailByBrKey', {params: {'brKey': brKey}, headers: getAuthHeader()});
    }
}
