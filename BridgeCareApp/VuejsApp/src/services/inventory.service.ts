import axios from 'axios';
import {Network} from '@/shared/models/iAM/network';
import {InventoryItem, InventoryItemDetail} from '@/shared/models/iAM/inventory';
import {mockInventory, mockInventoryItemDetail} from '@/shared/utils/mock-data';

axios.defaults.baseURL = process.env.VUE_APP_URL;

export default class InventoryService {
    getInventory(network: Network): Promise<InventoryItem[]> {
        //return Promise.resolve<InventoryItem[]>(mockInventory);
        // TODO: integrate axios web service call for inventory
        return axios.get('/api/section', {
            params: {
                NetworkId: network.networkId,
                NetworkName: network.networkName,
            }
        }).then((response) => {
            return response;
            }).catch(error => {
                return error.response;
            });
    }

    getInventoryItemDetail(inventoryItem: InventoryItem): Promise<InventoryItemDetail> {
        return Promise.resolve<InventoryItemDetail>(mockInventoryItemDetail);
        // TODO: integrate axios web service call for inventory item detail
    }
}
