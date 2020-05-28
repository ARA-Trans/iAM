import {AxiosPromise} from 'axios';
import {axiosInstance} from '@/shared/utils/axios-instance';
import {NetworkAttribute} from '@/shared/models/iAM/attribute';

export default class AttributeService {
    /**
     * Gets a list of attributes
     */
    static getAttributes(): AxiosPromise {
        return axiosInstance.get('/api/GetAttributes');
    }

    static getAttributeSelectValues(networkAttribute: NetworkAttribute): AxiosPromise {
        return axiosInstance.get('/api/GetAttributeSelectValues', {params: networkAttribute});
    }
}
