import axios, {AxiosPromise} from 'axios';

axios.defaults.baseURL = process.env.VUE_APP_URL;

export default class ReportsService {
    /**
     * Gets a simulation's detailed report
     * @param networkId
     * @param simulationId
     */
    getDetailedReport(networkId: number, simulationId: number): AxiosPromise<Blob> {
        return axios.post<Blob>('/api/DetailedReport', {NetworkId: networkId, SimulationId: simulationId});
    }

    /**
     * Gets a simulation's summary report
     * @param networkId
     * @param simulationId
     * @param networkName
     * @param simulationName
     */
    getSummaryReport(networkId: number, simulationId: number, networkName: string, simulationName: string): AxiosPromise<Blob> {
        return axios.post<Blob>(
            '/api/SummaryReport',
            {
                NetworkId: networkId,
                SimulationId: simulationId,
                NetworkName: networkName,
                SimulationName: simulationName
            }
        );
    }
}
