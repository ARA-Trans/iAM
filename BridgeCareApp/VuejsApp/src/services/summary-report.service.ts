import axios from 'axios';

axios.defaults.baseURL = process.env.VUE_APP_URL;

export default class SummaryReportService {
    getSummaryReport(networkId: number, simulationId: number, networkName: string, simulationName: string): Promise<Blob> {
        return axios({
            method: 'post',
            url: '/api/SummaryReport',
            responseType: 'blob',
            data: {
                NetworkId: networkId,
                SimulationId: simulationId,
                NetworkName: networkName,
                SimulationName: simulationName
            }
        }).then(response => {
            return new Blob([response.data]);
        });
    }
}
