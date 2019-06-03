import {AxiosPromise} from 'axios';
import {Network} from '@/shared/models/iAM/network';
import {axiosInstance} from '@/shared/utils/axios-instance';

export default class NetworkService {
    static getNetworks(): AxiosPromise<Network[]> {
        return axiosInstance.get<Network[]>('/api/Networks');
    }
}
