import {AxiosPromise} from 'axios';
import {axiosInstance} from '@/shared/utils/axios-instance';
import {Attribute} from '@/shared/models/iAM/attribute';

export default class AttributeService {
    /**
     * Gets a list of attributes
     */
    static getAttributes(): AxiosPromise<Attribute[]> {
        return axiosInstance.get<Attribute[]>('/api/GetAttributes');
    }
}
