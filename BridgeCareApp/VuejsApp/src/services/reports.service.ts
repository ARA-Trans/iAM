import {AxiosPromise} from 'axios';
import {axiosInstance} from '@/shared/utils/axios-instance';
import {emptyScenario, Scenario} from '@/shared/models/iAM/scenario';

export default class ReportsService {
    /**
     * Gets a scenario's detailed report
     * @param selectedScenarioData Scenario data to use in generating the report
     */
    static getDetailedReport(selectedScenarioData: Scenario): AxiosPromise<any> {
        return axiosInstance.post<any>('/api/DetailedReport', selectedScenarioData, {responseType: 'blob'});
    }

    /**
     * Gets a scenario's summary report
     * @param selectedScenarioData Scenario data to use in generating the report
     */
    static getSummaryReport(selectedScenarioData: Scenario): AxiosPromise<any> {
        return axiosInstance.post<any>('/api/SummaryReport', selectedScenarioData, {responseType: 'blob'});
    }
}
