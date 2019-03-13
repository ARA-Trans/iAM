import axios from 'axios';
import {Network} from '@/models/network';

axios.defaults.baseURL = process.env.VUE_APP_URL;

export default class NetworkService {
    getNetworks(): Promise<Network[]> {
        return axios.get('/api/Networks')
            .then(response => {
                return response.data as Promise<Network[]>;
            });
    }
}
