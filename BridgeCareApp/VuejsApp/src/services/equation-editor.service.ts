import {AxiosPromise} from 'axios';
import {axiosInstance} from '@/shared/utils/axios-instance';
// TODO: remove mockAdapter code when api is implemented
import {mockAdapterInstance} from '@/shared/utils/mock-adapter-instance';

export default class EquationEditorService {
    /**
     * Checks an equation's validity
     * @param equation The equation to check
     */
    static checkEquationValidity(equation: string): AxiosPromise<boolean> {
        mockAdapterInstance.onPost(`${axiosInstance.defaults.baseURL}/api/ValidateEquation`)
            .reply(200, true);
        return axiosInstance.post<boolean>('/api/ValidateEquation', equation);
    }
}