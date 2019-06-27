import {AxiosError, AxiosResponse} from 'axios';
import {hasValue} from '@/shared/utils/has-value-util';
import {prop} from 'ramda';

export const http2XX = /(2(0|2)[0-8])/;

export const setStatusMessage = (response: AxiosResponse) => {
    if (hasValue(response)) {
        return hasValue(response.statusText)
            ? `: HTTP Code ${response.status} => ${response.statusText}`
            : `: HTTP Code ${response.status}`;
    }

    return ': An unknown error occurred. Make sure you have internet connectivity then try again.';
};

export const getErrorMessage = (error: AxiosError) => {
    if (hasValue(error)) {
        if (hasValue(prop('response', error))) {
            const response: AxiosResponse = prop('response', error) as AxiosResponse;

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
