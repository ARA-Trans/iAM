import axios, {AxiosError, AxiosPromise, AxiosResponse} from 'axios';
import {hasValue} from '@/shared/utils/has-value-util';

axios.defaults.baseURL = process.env.VUE_APP_URL;

export default class SummaryReportService {
    getSummaryReport(networkId: number, simulationId: number, networkName: string, simulationName: string): Promise<Blob> {
        return axios.post<Blob>(
            '/api/SummaryReport',
            {
                NetworkId: networkId,
                SimulationId: simulationId,
                NetworkName: networkName,
                SimulationName: simulationName
            }
        ).then((response: AxiosResponse) => {
            /*if (hasValue(response) && /(2[01])\w+/g.test(response.status.toString())) {
                return new Blob([response.data]);
            }*/
            const axiosError: AxiosError = {
                config: response.config,
                name: '',
                message: 'Failed to get summary report data'
            };
            return Promise.reject<any>(axiosError);
        }).catch((response: AxiosError) => response);
    }
}
