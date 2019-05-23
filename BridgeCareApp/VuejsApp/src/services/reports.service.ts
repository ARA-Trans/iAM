import {AxiosPromise} from 'axios';
import {axiosInstance} from '@/shared/utils/axios-instance';

export default class ReportsService {
    /**
     * Gets a simulation's detailed report
     * @param networkId
     * @param simulationId
     */
    getDetailedReport(networkId: number, simulationId: number): AxiosPromise<Blob> {
        return axiosInstance.post<Blob>('/api/DetailedReport', {NetworkId: networkId, SimulationId: simulationId});
    }

    /**
     * Gets a simulation's summary report
     * @param networkId
     * @param simulationId
     * @param networkName
     * @param simulationName
     */
    getSummaryReport(networkId: number, simulationId: number, networkName: string, simulationName: string): AxiosPromise<Blob> {
        return axiosInstance.post<Blob>(
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
