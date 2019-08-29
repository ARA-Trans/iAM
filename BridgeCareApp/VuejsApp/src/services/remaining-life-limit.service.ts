import {axiosInstance, nodejsAxiosInstance} from '@/shared/utils/axios-instance';
import {AxiosPromise} from 'axios';
import {RemainingLifeLimit, RemainingLifeLimitLibrary} from '@/shared/models/iAM/remaining-life-limit';

const modifyDataForMongoDB = (remainingLifeLimitLibrary: RemainingLifeLimitLibrary): any => {
    const remainingLifeLimitLibraryData: any = {
        ...remainingLifeLimitLibrary,
        _id: remainingLifeLimitLibrary.id,
        remainingLifeLimits: remainingLifeLimitLibrary.remainingLifeLimits
            .map((remainingLifeLimit: RemainingLifeLimit) => {
                const remainingLifeLimitData: any = {...remainingLifeLimit, _id: remainingLifeLimit.id};
                delete remainingLifeLimitData.id;
                return remainingLifeLimitData;
            })
    };
    delete remainingLifeLimitLibraryData.id;
    return remainingLifeLimitLibraryData;
};

export default class RemainingLifeLimitService {
    /**
     * Gets all remaining life limit libraries
     */
    static getRemainingLifeLimitLibraries(): AxiosPromise {
        return nodejsAxiosInstance.get('/api/GetRemainingLifeLimitLibraries');
    }

    /**
     * Creates a remaining life limit library
     * @param createdRemainingLifeLimitLibrary The remaining life limit library create data
     */
    static createRemainingLifeLimitLibrary(createdRemainingLifeLimitLibrary: RemainingLifeLimitLibrary): AxiosPromise {
        return nodejsAxiosInstance.post('/api/CreateRemainingLifeLimitLibrary', modifyDataForMongoDB(createdRemainingLifeLimitLibrary));
    }

    /**
     * Updates a remaining life limit library
     * @param updatedRemainingLifeLimitLibrary The remaining life limit library update data
     */
    static updateRemainingLifeLimitLibrary(updatedRemainingLifeLimitLibrary: RemainingLifeLimitLibrary): AxiosPromise {
        return nodejsAxiosInstance.put('/api/UpdateRemainingLifeLimitLibrary', modifyDataForMongoDB(updatedRemainingLifeLimitLibrary));
    }

    /**
     * Gets a scenario's remaining life limit library data
     * @param selectedScenarioId Scenario id to use in finding a scenario's remaining life limit library data
     */
    static getScenarioRemainingLifeLimitLibrary(selectedScenarioId: number): AxiosPromise {
        return axiosInstance.get(`/api/GetScenarioRemainingLifeLimitLibrary/${selectedScenarioId}`);
    }

    /**
     * Upserts a scenario's remaining life limit library data
     * @param saveRemainingLifeLimitLibraryData The scenario remaining life limit library upsert data
     */
    static saveScenarioRemainingLifeLimitLibrary(saveRemainingLifeLimitLibraryData: RemainingLifeLimitLibrary): AxiosPromise {
        return axiosInstance.post('/api/SaveScenarioRemainingLifeLimitLibrary', saveRemainingLifeLimitLibraryData);
    }
}
