import {AxiosPromise} from 'axios';
import {axiosInstance} from '@/shared/utils/axios-instance';


export default class AuthenticationService {
    /**
     * Authenticates a user
     */
    static authenticateUser(): AxiosPromise {
        return axiosInstance.get('/auth/AuthenticateUser', {withCredentials: true});
    }
}
