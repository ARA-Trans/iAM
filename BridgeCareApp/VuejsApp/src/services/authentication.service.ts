import {AxiosPromise} from 'axios';
import {UserInformation} from '@/shared/models/iAM/user-information';
import {axiosInstance} from '@/shared/utils/axios-instance';


export default class AuthenticationService {
    /**
     * Authenticates a user
     */
    static authenticateUser(): AxiosPromise<UserInformation> {
        return axiosInstance.get<UserInformation>('/auth/AuthenticateUser', {withCredentials: true});
    }
}
