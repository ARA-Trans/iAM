import axios from 'axios';
import {Network} from '@/shared/models/iAM/network';
import {InventoryItem, InventoryItemDetail} from '@/shared/models/iAM/inventory';
import {mockInventory, mockInventoryItemDetail} from '@/shared/utils/mock-data';

axios.defaults.baseURL = process.env.VUE_APP_URL;

export default class InventoryService {
    getInventory(network: Network): Promise<InventoryItem[]> {
        return Promise.resolve<InventoryItem[]>(mockInventory);
        // TODO: integrate axios web service call for inventory
    }   

    getInventoryItemDetail(inventoryItem: InventoryItem): Promise<InventoryItemDetail> {
        // return Promise.resolve<InventoryItemDetail>(mockInventoryItemDetail);        

        return axios.get('/api/InventoryItemDetail', {
            headers: { 'Content-Type': 'application/json' },
            params: {
                "SectionId": inventoryItem.simulationId,
                "ReferenceId": inventoryItem.referenceId,
                "ReferenceKey": inventoryItem.referenceKey,
                "NetworkId": inventoryItem.networkId
            }
        }).then((response: any) => {
            return response.data as Promise<InventoryItemDetail>;
        });
    }
}
