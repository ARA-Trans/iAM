import axios from 'axios';
import {InventoryItem, InventoryItemDetail} from '@/shared/models/iAM/inventory';

axios.defaults.baseURL = process.env.VUE_APP_URL;

export default class InventoryService {
    getInventory(): Promise<InventoryItem[]> {       
        return axios.get('/api/InventorySelectionModels').then((response: any) => {
            return response.data as Promise<InventoryItem[]>;
        });
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
