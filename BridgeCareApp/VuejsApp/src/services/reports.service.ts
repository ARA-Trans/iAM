import {AxiosPromise} from 'axios';
import {axiosInstance} from '@/shared/utils/axios-instance';
import {Scenario} from '@/shared/models/iAM/scenario';
import {getAuthHeader} from '@/shared/utils/authentication-header';

export default class ReportsService {
    /**
     * Gets a scenario's detailed report
     * @param selectedScenarioData Scenario data to use in generating the report
     */
    static getDetailedReport(selectedScenarioData: Scenario): AxiosPromise {
        return axiosInstance.post('/api/GetDetailedReport', selectedScenarioData, {responseType: 'blob', headers: getAuthHeader()});
    }

    /**
     * Gets a scenario's summary report
     * @param selectedScenarioData Scenario data to use in generating the report
     */
    static getSummaryReport(selectedScenarioData: Scenario): AxiosPromise {
        return axiosInstance.post('/api/GetSummaryReport', selectedScenarioData, {responseType: 'blob', headers: getAuthHeader()});
    }

    static getSummaryReportMissingAttributes(selectedScenarioId: number, selectedNetworkId: number) {
        return axiosInstance
            .get(`/api/GetSummaryReportMissingAttributes?simulationId=${selectedScenarioId}&networkId=${selectedNetworkId}`, {headers: getAuthHeader()});
    }
}
