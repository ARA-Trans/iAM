import {AxiosResponse} from 'axios';
import {hasValue} from '@/shared/utils/has-value-util';

export const http2XX = /(2(0|2)[0-8])/;

export const setStatusMessage = (response: AxiosResponse) => {
    return hasValue(response.statusText)
        ? `: HTTP Code ${response.status} => ${response.statusText}`
        : `: HTTP Code ${response.status}`;
};
