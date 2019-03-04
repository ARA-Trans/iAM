import axios from 'axios'

axios.defaults.baseURL = process.env.VUE_APP_URL;

export default class DetailedReportService {
    getDetailedReport(networkId: number, simulationId: number): Promise<Blob> {
        return axios({
            method: 'post',
            url: '/api/DetailedReport',
            responseType: 'blob',
            data: {
                NetworkId: networkId,
                SimulationId: simulationId
            }
        }).then(response => {
            return new Blob([response.data]);
        });
    }
}
