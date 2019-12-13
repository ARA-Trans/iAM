import {AxiosPromise} from 'axios';
import {axiosInstance, nodejsAxiosInstance} from '@/shared/utils/axios-instance';
import {Deficient, DeficientLibrary} from '@/shared/models/iAM/deficient';
import {getAuthorizationHeader} from '@/shared/utils/authorization-header';

const modifyDataForMongoDB = (deficientLibrary: DeficientLibrary): any => {
    const deficientLibraryData: any = {
        ...deficientLibrary,
        _id: deficientLibrary.id,
        deficients: deficientLibrary.deficients.map((deficient: Deficient) => {
            const deficientData: any = {
                ...deficient,
                _id: deficient.id,
            };
            delete deficientData.id;
            return deficientData;
        })
    };
    delete deficientLibraryData.id;
    return deficientLibraryData;
};

export default class DeficientService {
    /**
     * Gets deficient libraries
     */
    static getDeficientLibraries(): AxiosPromise {
        return nodejsAxiosInstance.get('/api/GetDeficientLibraries', {headers: getAuthorizationHeader()});
    }

    /**
     * Creates a deficient library
     * @param createdDeficientLibrary The deficient library create data
     */
    static createDeficientLibrary(createdDeficientLibrary: DeficientLibrary): AxiosPromise {
        return nodejsAxiosInstance.post('/api/CreateDeficientLibrary', modifyDataForMongoDB(createdDeficientLibrary), {headers: getAuthorizationHeader()});
    }

    /**
     * Updates a deficient library
     * @param updatedDeficientLibrary The deficient library update data
     */
    static updateDeficientLibrary(updatedDeficientLibrary: DeficientLibrary): AxiosPromise {
        return nodejsAxiosInstance.put('/api/UpdateDeficientLibrary', modifyDataForMongoDB(updatedDeficientLibrary), {headers: getAuthorizationHeader()});
    }

    /**
     * Gets scenario deficient library data
     * @param selectedScenarioId Scenario object id
     */
    static getScenarioDeficientLibrary(selectedScenarioId: number): AxiosPromise {
        return axiosInstance.get(`/api/GetScenarioDeficientLibrary/${selectedScenarioId}`, {headers: getAuthorizationHeader()});
    }

    /**
     * Saves scenario deficient library data
     * @param scenarioDeficientLibraryData Scenario deficient library data
     */
    static saveScenarioDeficientLibrary(scenarioDeficientLibraryData: DeficientLibrary): AxiosPromise {
        return axiosInstance.post('/api/SaveScenarioDeficientLibrary', scenarioDeficientLibraryData, {headers: getAuthorizationHeader()});
    }
}