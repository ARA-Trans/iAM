import Vue from 'vue';
import axios from 'axios';
import {Network} from '@/models/network';
import {ISection, mockSections} from '@/models/section';

axios.defaults.baseURL = process.env.VUE_APP_URL;

export default class InventoryService extends Vue {
    getNetworkInventory(network: Network): Promise<ISection[]> {
        this.$store.dispatch({
            type: 'busy/setIsBusy',
            isBusy: true
        });
        return new Promise(() => {
            this.$store.dispatch({
                type: 'busy/setIsBusy',
                isBusy: false
            });
            return mockSections;
        });
        // TODO: integrate axios web service call for network inventory
    }
}
