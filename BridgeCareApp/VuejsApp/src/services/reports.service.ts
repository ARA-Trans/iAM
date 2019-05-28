import {AxiosPromise} from 'axios';
import {axiosInstance} from '@/shared/utils/axios-instance';
import {emptyScenario, Scenario} from '@/shared/models/iAM/scenario';

export default class ReportsService {
    /**
     * Gets a scenario's detailed report
     * @param networkId Scenario's network id
     * @param simulationId Scenario's simulation id
     */
    static getDetailedReport(networkId: number, simulationId: number): AxiosPromise<Blob> {
        const scenario: Scenario = {
            ...emptyScenario,
            networkId: networkId,
            simulationId: simulationId
        };
        return axiosInstance.post<Blob>('/api/DetailedReport', scenario);
    }

    /**
     * Gets a scenario's summary report
     * @param networkId Scenario's network id
     * @param simulationId Scenario's simulation id
     */
    static getSummaryReport(networkId: number, simulationId: number): AxiosPromise<Blob> {
        const scenario: Scenario = {
            ...emptyScenario,
            networkId: networkId,
            simulationId: simulationId
        };
        return axiosInstance.post<Blob>('/api/SummaryReport', scenario);
    }
}
