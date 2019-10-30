import {AxiosPromise} from 'axios';
import {axiosInstance, nodejsAxiosInstance} from '@/shared/utils/axios-instance';
import {Target, TargetLibrary} from '@/shared/models/iAM/target';

const modifyDataForMongoDB = (targetLibrary: TargetLibrary): any => {
    const targetLibraryData: any = {
        ...targetLibrary,
        _id: targetLibrary.id,
        targets: targetLibrary.targets.map((target: Target) => {
            const targetData: any = {
                ...target,
                _id: target.id
            };
            delete targetData.id;
            return targetData;
        })
    };
    delete targetLibraryData.id;
    return targetLibraryData;
};

export default class TargetService {
    /**
     * Gets target libraries data
     */
    static getTargetLibraries(): AxiosPromise {
        return nodejsAxiosInstance.get('/api/GetTargetLibraries');
    }

    /**
     * Created a target library
     * @param createdTargetLibrary The target library create data
     */
    static createTargetLibrary(createdTargetLibrary: TargetLibrary): AxiosPromise {
        return nodejsAxiosInstance.post('/api/CreateTargetLibrary', modifyDataForMongoDB(createdTargetLibrary));
    }

    /**
     * Updates a target library
     * @param updatedTargetLibrary The target library update data
     */
    static updateTargetLibrary(updatedTargetLibrary: TargetLibrary): AxiosPromise {
        return nodejsAxiosInstance.put('/api/UpdateTargetLibrary', modifyDataForMongoDB(updatedTargetLibrary));
    }

    /**
     * Gets a scenario's target library data
     * @param selectedScenarioId Scenario object id
     */
    static getScenarioTargetLibrary(selectedScenarioId: number): AxiosPromise {
        return axiosInstance.get(`/api/GetScenarioTargetLibrary/${selectedScenarioId}`);
    }

    /**
     * Upserts a scenario's target library data
     * @param scenarioTargetLibraryData Scenario target library upsert data
     */
    static saveScenarioTargetLibrary(scenarioTargetLibraryData: TargetLibrary): AxiosPromise {
        return axiosInstance.post('/api/SaveScenarioTargetLibrary', scenarioTargetLibraryData);
    }
}