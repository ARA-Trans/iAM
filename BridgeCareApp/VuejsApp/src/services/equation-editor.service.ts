import {AxiosPromise} from 'axios';
import {axiosInstance} from '@/shared/utils/axios-instance';
import {EquationValidation} from '@/shared/models/iAM/equation-validation';

export default class EquationEditorService {
    /**
     * Checks an equation's validity
     * @param equationValidation Equation info to validate
     */
    static checkEquationValidity(equationValidation: EquationValidation): AxiosPromise {
        return axiosInstance.post('/api/ValidateEquation', equationValidation);
    }
}
