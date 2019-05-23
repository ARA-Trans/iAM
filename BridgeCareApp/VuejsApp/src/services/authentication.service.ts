import {AxiosPromise} from 'axios';
import {UserInformation} from '@/shared/models/iAM/user-information';
import {axiosInstance} from '@/shared/utils/axios-instance';
// TODO: remove mockAdapter code when api is implemented
import {mockAdapterInstance} from '@/shared/utils/mock-adapter-instance';
import {reject} from 'q';


export default class AuthenticationService {
    /**
     * Authenticates a user
     */
    static authenticateUser(): AxiosPromise<UserInformation> {
        // TODO: remove mockAdapter code when api is implemented
        mockAdapterInstance.onGet(`${axiosInstance.defaults.baseURL}/auth/AuthenticateUser`)
            .reply(200, {name: 'John Smith', id: '0'} as UserInformation);
        return axiosInstance.get<UserInformation>('/auth/AuthenticateUser', {withCredentials: true})
            .catch((error: any) => reject(error));
    }
}
