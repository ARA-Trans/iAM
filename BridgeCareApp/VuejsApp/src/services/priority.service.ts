import {AxiosPromise} from 'axios';
import {axiosInstance, nodejsAxiosInstance} from '@/shared/utils/axios-instance';
import {Priority, PriorityFund, PriorityLibrary} from '@/shared/models/iAM/priority';

const modifyDataForMongoDB = (priorityLibrary: PriorityLibrary): any => {
    const priorityLibraryData: any = {
        ...priorityLibrary,
        _id: priorityLibrary.id,
        priorities: priorityLibrary.priorities.map((priority: Priority) => {
            const priorityData: any = {
                ...priority,
                _id: priority.id,
                priorityFunds: priority.priorityFunds.map((priorityFund: PriorityFund) => {
                    const priorityFundData: any = {
                        ...priorityFund,
                        _id: priorityFund.id
                    };
                    delete priorityFundData.id;
                    return priorityFundData;
                })
            };
            delete priorityData.id;
            return priorityData;
        })
    };
    delete priorityLibraryData.id;
    return priorityLibraryData;
};

export default class PriorityService {
    /**
     * Gets priority libraries data
     */
    static getPriorityLibraries(): AxiosPromise {
        return nodejsAxiosInstance.get('/api/GetPriorityLibraries');
    }

    /**
     * Creates a priority library
     * @param createdPriorityLibrary The priority library create data
     */
    static createPriorityLibrary(createdPriorityLibrary: PriorityLibrary): AxiosPromise {
        return nodejsAxiosInstance.post('/api/CreatePriorityLibrary', modifyDataForMongoDB(createdPriorityLibrary));
    }

    /**
     * Updates a priority library
     * @param updatedPriorityLibrary The priority library update data
     */
    static updatePriorityLibrary(updatedPriorityLibrary: PriorityLibrary): AxiosPromise {
        return nodejsAxiosInstance.put('/api/UpdatePriorityLibrary', modifyDataForMongoDB(updatedPriorityLibrary));
    }

    /**
     * Gets a scenario's priority library data
     * @param selectedScenarioId Scenario id to use in finding a scenario's priority library data
     */
    static getScenarioPriorityLibrary(selectedScenarioId: number): AxiosPromise {
        return axiosInstance.get(`/api/GetScenarioPriorityLibrary/${selectedScenarioId}`);
    }

    /**
     * Upserts a scenario's priority library data
     * @param saveScenarioPriorityLibraryData The scenario priority library upsert data
     */
    static saveScenarioPriorityLibrary(saveScenarioPriorityLibraryData: PriorityLibrary): AxiosPromise {
        return axiosInstance.post('/api/SaveScenarioPriorityLibrary', saveScenarioPriorityLibraryData);
    }
}