import {AxiosPromise, AxiosResponse} from 'axios';
import {axiosInstance} from '@/shared/utils/axios-instance';
import { hasValue } from '@/shared/utils/has-value-util';


export default class AuthenticationService {
    static getUserTokens(code: string): AxiosPromise {
        return new Promise<AxiosResponse> ((resolve) => {
            axiosInstance.get(`/auth/UserTokens/${code}`)
            .then((response: AxiosResponse<string>) => {
                if (hasValue(response, 'data')) {
                    return resolve(response);
                }
            })
            .catch((err: any) => {
                return resolve(err);
            });
        });
    }

    static getUserInfo(token: string): AxiosPromise {
        return new Promise<AxiosResponse> ((resolve) => {
            axiosInstance.get(`/auth/UserInfo/${token}`)
            .then((response: AxiosResponse<string>) => {
                if (hasValue(response, 'data')) {
                    return resolve(response);
                }
            })
            .catch((err: any) => {
                return resolve(err);
            });
        });
    }

    static revokeToken(token: string): AxiosPromise {
        return new Promise<AxiosResponse> ((resolve) => {
            axiosInstance.post(`/auth/RevokeToken/${token}`)
            .then((response: AxiosResponse<any>) => {
                return resolve(response);
            })
            .catch((err: any) => {
                return resolve(err);
            });
        });
    }
}
