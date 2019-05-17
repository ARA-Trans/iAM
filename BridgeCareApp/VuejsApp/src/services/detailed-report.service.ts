import axios, {AxiosResponse} from 'axios';

axios.defaults.baseURL = process.env.VUE_APP_URL;

export default class DetailedReportService {
    getDetailedReport(networkId: number, simulationId: number): Promise<Blob> {
        return axios.post<Blob>('/api/DetailedReport',
            {
                NetworkId: networkId,
                SimulationId: simulationId
            }
        ).then((response: AxiosResponse) => new Blob([response.data]));
    }
}
