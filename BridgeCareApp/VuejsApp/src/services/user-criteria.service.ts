import {AxiosPromise} from 'axios';
import {UserCriteria} from '@/shared/models/iAM/user-criteria';
import {axiosInstance} from '@/shared/utils/axios-instance';

export default class UserCriteriaService {

    /**
     * Gets the criteria for the current user
     */
    static getOwnUserCriteria() : AxiosPromise {
        return axiosInstance.get('/api/GetUserCriteria');
    }

    /**
     * Gets the criteria for all users
     */
    static getAllUserCriteria() : AxiosPromise {
        return axiosInstance.get('/api/GetAllUserCriteria');
    }

    /**
     * Sets the criteria for a user
     */
    static setUserCriteria(userCriteria: UserCriteria) : AxiosPromise {
        return axiosInstance.post('/api/SetUserCriteria', userCriteria);
    }
}