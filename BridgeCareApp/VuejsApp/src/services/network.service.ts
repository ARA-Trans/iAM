import {AxiosPromise} from 'axios';
import {axiosInstance} from '@/shared/utils/axios-instance';
import {getAuthHeader} from '@/shared/utils/authentication-header';

export default class NetworkService {
    static getNetworks(): AxiosPromise {
        return axiosInstance.get('/api/GetNetworks', {headers: getAuthHeader()});
    }
}
