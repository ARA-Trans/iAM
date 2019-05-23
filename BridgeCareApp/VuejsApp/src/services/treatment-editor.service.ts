import {AxiosPromise} from 'axios';
import {TreatmentLibrary} from '@/shared/models/iAM/treatment';
import {axiosInstance} from '@/shared/utils/axios-instance';
// TODO: remove mockAdapter code when api is implemented
import {mockAdapterInstance} from '@/shared/utils/mock-adapter-instance';
import {mockScenarioTreatmentLibrary, mockTreatmentLibraries} from '@/shared/utils/mock-data';

export default class TreatmentEditorService {

    /**
     * Gets all treatment libraries
     */
    static getTreatmentLibraries(): AxiosPromise<TreatmentLibrary[]> {
        // TODO: remove mockAdapter code when api is implemented
        mockAdapterInstance.onGet(`${axiosInstance.defaults.baseURL}/api/GetTreatmentLibraries`)
            .reply(200, mockTreatmentLibraries);
        return axiosInstance.get<TreatmentLibrary[]>('/api/GetTreatmentLibraries');
    }

    /**
     * Creates a treatment library
     * @param createTreatmentLibraryData The treatment library create data
     */
    static createTreatmentLibrary(createTreatmentLibraryData: TreatmentLibrary): AxiosPromise<TreatmentLibrary> {
        // TODO: remove mockAdapter code when api is implemented
        mockAdapterInstance.onPost(`${axiosInstance.defaults.baseURL}/api/CreateTreatmentLibrary`)
            .reply(200, createTreatmentLibraryData);
        return axiosInstance.post<TreatmentLibrary>('/api/CreateTreatmentLibrary', createTreatmentLibraryData);
    }

    /**
     * Updates a treatment library
     * @param updateTreatmentLibraryData The treatment library update data
     */
    static updateTreatmentLibrary(updateTreatmentLibraryData: TreatmentLibrary): AxiosPromise<TreatmentLibrary> {
        // TODO: remove mockAdapter code when api is implemented
        mockAdapterInstance.onPost(`${axiosInstance.defaults.baseURL}/api/UpdateTreatmentLibrary`)
            .reply(200, updateTreatmentLibraryData);
        return axiosInstance.post<TreatmentLibrary>('/api/UpdateTreatmentLibrary', updateTreatmentLibraryData);
    }

    /**
     * Gets a scenario's treatment library data
     * @param selectedScenarioId Scenario object id
     */
    static getScenarioTreatmentLibrary(selectedScenarioId: number): AxiosPromise<TreatmentLibrary> {
        // TODO: remove mockAdapter code when api is implemented
        mockAdapterInstance.onGet(`${axiosInstance.defaults.baseURL}/api/GetScenarioTreatmentLibrary`)
            .reply(200, mockScenarioTreatmentLibrary);
        return axiosInstance.get<TreatmentLibrary>('/api/GetScenarioTreatmentLibrary', {
            params: {'selectedScenarioId': selectedScenarioId}
        });
    }

    /**
     * Saves a scenario's treatment library data
     * @param saveScenarioTreatmentLibraryData The scenario treatment library save data
     */
    static saveScenarioTreatmentLibrary(saveScenarioTreatmentLibraryData: TreatmentLibrary): AxiosPromise<TreatmentLibrary> {
        // TODO: remove mockAdapter code when api is implemented
        mockAdapterInstance.onPost(`${axiosInstance.defaults.baseURL}/api/SaveScenarioTreatmentLibrary`)
            .reply(200, saveScenarioTreatmentLibraryData);
        return axiosInstance.post<TreatmentLibrary>('/api/SaveScenarioTreatmentLibrary', saveScenarioTreatmentLibraryData);
    }
}