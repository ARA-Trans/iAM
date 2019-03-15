import axios from 'axios';
import {Network} from '@/shared/models/iAM/network';
import {Section} from '@/shared/models/iAM/section';
import {mockSections} from '@/shared/utils/mock-data';

axios.defaults.baseURL = process.env.VUE_APP_URL;

export default class InventoryService {
    getNetworkInventory(network: Network): Promise<Section[]> {
        return Promise.resolve<Section[]>(mockSections);
        // TODO: integrate axios web service call for network inventory
    }
}
