import axios from 'axios';
import {Network} from '@/shared/models/iAM/network';
import {InventoryItem, InventoryItemDetail} from '@/shared/models/iAM/inventory';
import {mockInventory, mockInventoryItemDetail} from '@/shared/utils/mock-data';

axios.defaults.baseURL = process.env.VUE_APP_URL;

export default class InventoryService {
    getInventory(): Promise<InventoryItem[]> {
        return Promise.resolve<InventoryItem[]>(mockInventory);
        // TODO: integrate axios web service call for inventory
    }   

    getInventoryItemDetailByBMSId(bmsId: number): Promise<InventoryItemDetail> {
        return axios.get('/api/InventoryItemDetailByBMSId', {
            headers: { 'Content-Type': 'application/json' },
            params: {
                'bmsId': bmsId
            }
        }).then((response: any) => {
            return response.data as Promise<InventoryItemDetail>;
        });
    }

    getInventoryItemDetailByBRKey(brKey: number): Promise<InventoryItemDetail> {
        return axios.get('/api/InventoryItemDetailByBRKey', {
            headers: { 'Content-Type': 'application/json' },
            params: {
                'brKey': brKey
            }
        }).then((response: any) => {
            return response.data as Promise<InventoryItemDetail>;
        });
    }
}
