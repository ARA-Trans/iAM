import {AxiosPromise} from 'axios';
import {axiosInstance} from '@/shared/utils/axios-instance';
// TODO: remove mockAdapter code when api is implemented
import {mockAttributes} from '@/shared/utils/mock-data';
import {mockAdapterInstance} from '@/shared/utils/mock-adapter-instance';

export default class AttributeService {
    /**
     * Gets a list of attributes
     */
    static getAttributes(): AxiosPromise<string[]> {
        // TODO: remove mockAdapter code when api is implemented
        mockAdapterInstance.onGet(`${axiosInstance.defaults.baseURL}/api/GetAttributes`)
            .reply(200, mockAttributes);
        return axiosInstance.get<string[]>('/api/GetAttributes');
    }
}
