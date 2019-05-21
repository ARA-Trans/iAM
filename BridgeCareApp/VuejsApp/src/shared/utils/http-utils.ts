import {AxiosResponse} from 'axios';
import {hasValue} from '@/shared/utils/has-value-util';

export const http200Response = /(2[01])\w+/g;

export const setStatusMessage = (response: AxiosResponse) => {
    return hasValue(response.statusText)
        ? `: HTTP Code ${response.status} => ${response.statusText}`
        : `: HTTP Code ${response.status}`;
};
