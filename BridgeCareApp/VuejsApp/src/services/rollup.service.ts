import { AxiosPromise, AxiosResponse } from 'axios';
import { axiosInstance, nodejsAxiosInstance } from '@/shared/utils/axios-instance';
import { hasValue } from '@/shared/utils/has-value-util';
import { any, propEq } from 'ramda';
import { Rollup } from '../shared/models/iAM/rollup';

export default class RollupService {
    static getMongoRollups(): AxiosPromise {
        return new Promise<AxiosResponse<Rollup[]>>((resolve) => {
            nodejsAxiosInstance.get('api/GetMongoRollups')
                .then((response: AxiosResponse<Rollup[]>) => {
                    if (hasValue(response)) {
                        return resolve(response);
                    }
                })
                .catch((error: any) => {
                    return resolve(error.response);
                });
        });
    }

    static rollupNetwork(selectedNetwork: Rollup): AxiosPromise {
        return axiosInstance.post('/api/RunRollup', selectedNetwork);
    }
}