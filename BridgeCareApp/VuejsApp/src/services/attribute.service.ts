import {AxiosPromise} from 'axios';
import {axiosInstance} from '@/shared/utils/axios-instance';
import {NetworkAttributes} from '@/shared/models/iAM/attribute';

export default class AttributeService {
    /**
     * Gets a list of attributes
     */
    static getAttributes(): AxiosPromise {
        return axiosInstance.get('/api/GetAttributes');
    }

    static getAttributeSelectValues(networkAttributes: NetworkAttributes): AxiosPromise {
        return axiosInstance.post('/api/GetAttributesSelectValues', networkAttributes);
    }
}
