import {AxiosPromise} from 'axios';
import {axiosInstance, nodejsAxiosInstance} from '@/shared/utils/axios-instance';
import {CriteriaValidation} from '@/shared/models/iAM/criteria-validation';
import {CriteriaLibrary} from '@/shared/models/iAM/criteria';
import {convertFromVueToMongo} from '@/shared/utils/mongo-model-conversion-utils';

export default class CriteriaEditorService {
    /**
     * Checks a criteria's validity
     * @param criteria Criteria string to validate
     */
    static checkCriteriaValidity(criteriaValidation: CriteriaValidation): AxiosPromise {
        return axiosInstance.post('/api/ValidateCriteria', criteriaValidation);
    }

    static getCriteriaLibraries(): AxiosPromise {
        return nodejsAxiosInstance.get('/api/GetCriteriaLibraries');
    }

    static createCriteriaLibrary(criteriaLibrary: CriteriaLibrary): AxiosPromise {
        return nodejsAxiosInstance.post('/api/CreateCriteriaLibrary', convertFromVueToMongo(criteriaLibrary));
    }

    static updateCriteriaLibrary(criteriaLibrary: CriteriaLibrary): AxiosPromise {
        return nodejsAxiosInstance.put('/api/UpdateCriteriaLibrary', convertFromVueToMongo(criteriaLibrary));
    }

    static deleteCriteriaLibrary(criteriaLibraryId: string): AxiosPromise {
        return nodejsAxiosInstance.delete(`/api/DeleteCriteriaLibrary/${criteriaLibraryId}`);
    }
}
