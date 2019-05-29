import {AxiosPromise} from 'axios';
import {UserInformation} from '@/shared/models/iAM/user-information';
import {axiosInstance} from '@/shared/utils/axios-instance';
// TODO: remove MockAdapter code when api is implemented
import MockAdapter from 'axios-mock-adapter';


export default class AuthenticationService {
    /**
     * Authenticates a user
     */
    static authenticateUser(): AxiosPromise<UserInformation> {
        // TODO: remove MockAdapter code when api is implemented
        const mockAdapterInstance = new MockAdapter(axiosInstance)
            .onGet(`${axiosInstance.defaults.baseURL}/auth/AuthenticateUser`)
            .reply((config: any) => {
                mockAdapterInstance.restore();
                return [200, {name: 'John Smith', id: '0'} as UserInformation];
            });
        return axiosInstance.get<UserInformation>('/auth/AuthenticateUser', {withCredentials: true});
    }
}
