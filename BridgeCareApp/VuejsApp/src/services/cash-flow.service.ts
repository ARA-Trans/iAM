import {axiosInstance, nodejsAxiosInstance} from '@/shared/utils/axios-instance';
import {AxiosPromise} from 'axios';
import {CashFlowDuration, CashFlowLibrary, CashFlowParameter} from '@/shared/models/iAM/cash-flow';

const modifyDataForMongoDB = (cashFlowLibrary: CashFlowLibrary): any => {
    const cashFlowLibraryData: any = {
        ...cashFlowLibrary,
        _id: cashFlowLibrary.id,
        parameters: cashFlowLibrary.parameters.map((parameter: CashFlowParameter) => {
            const cashFlowParameterData: any = {...parameter, _id: parameter.id};
            delete cashFlowParameterData.id;
            return cashFlowParameterData;
        }),
        durations: cashFlowLibrary.durations.map((duration: CashFlowDuration) => {
            const cashFlowDurationData: any = {...duration, _id: duration.id};
            delete cashFlowDurationData.id;
            return cashFlowDurationData;
        })
    };
    delete cashFlowLibraryData.id;
    return cashFlowLibraryData;
};

export default class CashFlowService {
    /**
     * Gets all cash flow libraries
     */
    static getCashFlowLibraries(): AxiosPromise {
        return nodejsAxiosInstance.get('/api/GetCashFlowLibraries');
    }

    /**
     * Creates a cash flow library
     * @param createdCashFlowLibrary Cash flow library create data
     */
    static createCashFlowLibrary(createdCashFlowLibrary: CashFlowLibrary): AxiosPromise {
        return nodejsAxiosInstance.post('/api/CreateCashFlowLibrary', modifyDataForMongoDB(createdCashFlowLibrary));
    }

    /**
     * Updates a cash flow library
     * @param updatedCashFlowLibrary Cash flow library update data
     */
    static updateCashFlowLibrary(updatedCashFlowLibrary: CashFlowLibrary): AxiosPromise {
        return nodejsAxiosInstance.put('/api/UpdateCashFlowLibrary', modifyDataForMongoDB(updatedCashFlowLibrary));
    }

    /**
     * Gets a scenario's cash flow library data
     * @param selectedScenarioId Selected scenario's identifier
     */
    static getScenarioCashFlowLibrary(selectedScenarioId: number): AxiosPromise {
        return axiosInstance.get(`/api/GetScenarioCashFlowLibrary/${selectedScenarioId}`);
    }

    /**
     * Upserts a scenario's cash flow library data
     * @param saveScenarioCashFlowLibraryData Scenario's cash flow library upsert data
     */
    static saveScenarioCashFlowLibrary(saveScenarioCashFlowLibraryData: CashFlowLibrary): AxiosPromise {
        return axiosInstance.post('/api/SaveScenarioCashFlowLibrary', saveScenarioCashFlowLibraryData);
    }
}