import {AxiosResponse} from 'axios';
import {hasValue} from '@/shared/utils/has-value-util';

export const http2XX = /(2(0|2)[0-8])/;

export const setStatusMessage = (response: AxiosResponse) => {
    if (hasValue(response)) {
        return hasValue(response.statusText)
            ? `: HTTP Code ${response.status} => ${response.statusText}`
            : `: HTTP Code ${response.status}`;
    }

    return ': An unknown error occurred. Make sure you have internet connectivity then try again.';
};
