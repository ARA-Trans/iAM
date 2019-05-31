import {AxiosPromise} from 'axios';
import {axiosInstance} from '@/shared/utils/axios-instance';
// TODO: remove MockAdapter code when api is implemented
import {mockAdapter, mockAxiosInstance} from '@/shared/utils/axios-mock-adapter-instance';

export default class EquationEditorService {
    /**
     * Checks an equation's validity
     * @param equation The equation to check
     */
    static checkEquationValidity(equation: string): AxiosPromise<boolean> {
        // TODO: remove MockAdapter code when api is implemented
        mockAdapter
            .onPost('/api/ValidateEquation')
            .reply((config: any) => {
                return [200, true];
            });
        // TODO: replace mockAxiosInstance with axiosInstance
        return mockAxiosInstance.post<boolean>('/api/ValidateEquation', equation);
    }
}