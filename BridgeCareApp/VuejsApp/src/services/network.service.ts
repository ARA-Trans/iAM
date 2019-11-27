import {AxiosPromise} from 'axios';
import {axiosInstance} from '@/shared/utils/axios-instance';
import {getAuthorizationHeader} from '@/shared/utils/authorization-header';

export default class NetworkService {
    static getNetworks(): AxiosPromise {
        return axiosInstance.get('/api/GetNetworks', {headers: getAuthorizationHeader()});
    }
}
