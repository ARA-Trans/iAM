import {AxiosPromise} from 'axios';
import {axiosInstance} from '@/shared/utils/axios-instance';
import {getAuthHeader} from '@/shared/utils/authentication-header';

export default class AttributeService {
    /**
     * Gets a list of attributes
     */
    static getAttributes(): AxiosPromise {
        return axiosInstance.get('/api/GetAttributes', {headers: getAuthHeader()});
    }
}
