import {AxiosPromise} from 'axios';
import {axiosInstance} from '@/shared/utils/axios-instance';
import {getAuthorizationHeader} from '@/shared/utils/authorization-header';

export default class AttributeService {
    /**
     * Gets a list of attributes
     */
    static getAttributes(): AxiosPromise {
        return axiosInstance.get('/api/GetAttributes', {headers: getAuthorizationHeader()});
    }
}
