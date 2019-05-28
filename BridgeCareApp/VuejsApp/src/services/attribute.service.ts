import {AxiosPromise} from 'axios';
import {axiosInstance} from '@/shared/utils/axios-instance';
// TODO: remove MockAdapter code when api is implemented
import MockAdapter from 'axios-mock-adapter';
import {mockAttributes} from '@/shared/utils/mock-data';

export default class AttributeService {
    /**
     * Gets a list of attributes
     */
    static getAttributes(): AxiosPromise<string[]> {
        // TODO: remove MockAdapter code when api is implemented
        const mockAdapterInstance = new MockAdapter(axiosInstance)
            .onGet(`${axiosInstance.defaults.baseURL}/api/GetAttributes`)
            .reply((config: any) => {
                mockAdapterInstance.restore();
                return [200, mockAttributes];
            });
        return axiosInstance.get<string[]>('/api/GetAttributes');
    }
}
