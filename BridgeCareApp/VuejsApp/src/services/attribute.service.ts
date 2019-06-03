import {AxiosPromise} from 'axios';
import {axiosInstance} from '@/shared/utils/axios-instance';
import {AttributeName} from '@/shared/models/iAM/attribute-name';

export default class AttributeService {
    /**
     * Gets a list of attributes
     */
    static getAttributes(): AxiosPromise<AttributeName[]> {
        return axiosInstance.get<AttributeName[]>('/api/AttributeNames');
    }
}
