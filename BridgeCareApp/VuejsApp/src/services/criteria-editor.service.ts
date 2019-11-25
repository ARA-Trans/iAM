import {AxiosPromise} from 'axios';
import {axiosInstance} from '@/shared/utils/axios-instance';
import {CriteriaValidation} from '@/shared/models/iAM/criteria-validation';
import {getAuthHeader} from '@/shared/utils/authentication-header';

export default class CriteriaEditorService {
    /**
     * Checks a criteria's validity
     * @param criteria Criteria string to validate
     */
    static checkCriteriaValidity(criteriaValidation: CriteriaValidation): AxiosPromise {
        return axiosInstance.post('/api/ValidateCriteria', criteriaValidation, {headers: getAuthHeader()});
    }
}
