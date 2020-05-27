import {AxiosError, AxiosResponse} from 'axios';
import {hasValue} from '@/shared/utils/has-value-util';
import {prop} from 'ramda';
import {UserTokens} from '@/shared/models/iAM/authentication';

export const http2XX = /(2(0|2)[0-8])/;

export const setStatusMessage = (response: AxiosResponse) => {
    if (hasValue(response)) {
        return hasValue(response.statusText)
            ? `: HTTP Code ${response.status} => ${response.statusText}`
            : `: HTTP Code ${response.status}`;
    }

    return ': An unknown error occurred. Make sure you have internet connectivity then try again.';
};

export const setContentTypeCharset = (headers: any) => {
    if (headers) {
        if (headers['common']) {
            if (headers['common']['Content-Type']) {
                headers['common']['Content-Type'] = `${headers['common']['Content-Type']}; charset=utf-8`;
            } else {
                headers['common']['Content-Type'] = 'charset=utf-8';
            }
        } else if (headers['Content-Type']) {
            if (!headers['Content-Type'].match(/charset=utf-8/gi)) {
                headers['Content-Type'] = `${headers['Content-Type']}; charset=utf-8`;
            }
        } else if (headers['content-type']) {
            if (!headers['content-type'].match(/charset=utf-8/gi)) {
                headers['content-type'] = `${headers['content-type']}; charset=utf-8`;
            }
        } else {
            headers['Content-Type'] = 'charset=utf-8';
        }
    }

    return headers;
};

export const setAuthHeader = (headers: any) => {
    if (headers && localStorage.getItem('UserTokens')) {
        const userTokens: UserTokens = JSON.parse(localStorage.getItem('UserTokens') as string) as UserTokens;
        headers['Authorization'] = `Bearer ${userTokens.id_token}`;
    }

    return headers;
};

export const getErrorMessage = (error: AxiosError) => {
    if (hasValue(error)) {
        if (hasValue(prop('response', error))) {
            const response: AxiosResponse = prop('response', error) as AxiosResponse;

            if (hasValue(prop('statusText', response))) {
                return prop('statusText', response) as string;
            }

            if (hasValue(prop('data', response))) {
                const responseData: any = prop('data', response);

                if (hasValue(prop('exceptionMessage', responseData))) {
                    return prop('exceptionMessage', responseData) as string;
                }

                if (hasValue(prop('message', responseData))) {
                    return prop('message', responseData) as string;
                }
            }
        }

        if (hasValue(prop('message', error))) {
            return prop('message', error) as string;
        }
    }

    return 'An unknown error has occurred. Please try again later.';
};
