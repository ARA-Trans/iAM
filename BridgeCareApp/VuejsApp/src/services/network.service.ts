import {AxiosPromise} from 'axios';
import {axiosInstance} from '@/shared/utils/axios-instance';

export default class NetworkService {
    static getNetworks(): AxiosPromise {
        return axiosInstance.get('/api/Networks');
    }
}
