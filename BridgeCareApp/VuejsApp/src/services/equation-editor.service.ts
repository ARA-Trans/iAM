import {AxiosPromise} from 'axios';
import {axiosInstance} from '@/shared/utils/axios-instance';
// TODO: remove MockAdapter code when api is implemented
import MockAdapter from 'axios-mock-adapter';

export default class EquationEditorService {
    /**
     * Checks an equation's validity
     * @param equation The equation to check
     */
    static checkEquationValidity(equation: string): AxiosPromise<boolean> {
        // TODO: remove MockAdapter code when api is implemented
        const mockAdapterInstance = new MockAdapter(axiosInstance)
            .onPost(`${axiosInstance.defaults.baseURL}/api/ValidateEquation`)
            .reply((config: any) => {
                mockAdapterInstance.restore();
                return [200, true];
            });
        return axiosInstance.post<boolean>('/api/ValidateEquation', equation);
    }
}