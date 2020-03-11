import {AxiosPromise} from 'axios';
import {axiosInstance} from '@/shared/utils/axios-instance';
import {Equation} from '@/shared/models/iAM/equation';

export default class EquationEditorService {
    /**
     * Checks an equation's validity
     * @param equationValidation Equation info to validate
     */
    static checkEquationValidity(equationValidation: Equation): AxiosPromise {
        return axiosInstance.post('/api/ValidateEquation', equationValidation);
    }
}
