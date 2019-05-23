import {AxiosPromise} from 'axios';
import {PerformanceLibrary} from '@/shared/models/iAM/performance';
import {axiosInstance} from '@/shared/utils/axios-instance';
// TODO: remove mockAdapter code when api is implemented
import {mockAdapterInstance} from '@/shared/utils/mock-adapter-instance';
import {mockPerformanceLibraries, mockScenarioPerformanceLibrary} from '@/shared/utils/mock-data';

export default class PerformanceEditorService {
    /**
     * Gets all performance Libraries a user can read/edit
     */
    static getPerformanceLibraries(): AxiosPromise<PerformanceLibrary[]> {
        // TODO: remove mockAdapter code when api is implemented
        mockAdapterInstance.onGet(`${axiosInstance.defaults.baseURL}/api/GetPerformanceLibraries`)
            .reply(200, mockPerformanceLibraries);
        return axiosInstance.get<PerformanceLibrary[]>('/api/GetPerformanceLibraries');
    }

    /**
     * Creates a performance library
     * @param createPerformanceLibraryData The performance library create data
     */
    static createPerformanceLibrary(createPerformanceLibraryData: PerformanceLibrary): AxiosPromise<PerformanceLibrary> {
        // TODO: remove mockAdapter code when api is implemented
        mockAdapterInstance.onPost(`${axiosInstance.defaults.baseURL}/api/CreatePerformanceLibrary`)
            .reply(200, createPerformanceLibraryData);
        return axiosInstance.post<PerformanceLibrary>('/api/CreatePerformanceLibrary', createPerformanceLibraryData);
    }

    /**
     * Updates a performance library
     * @param updatePerformanceLibraryData The performance library update data
     */
    static updatePerformanceLibrary(updatePerformanceLibraryData: PerformanceLibrary): AxiosPromise<PerformanceLibrary> {
        // TODO: remove mockAdapter code when api is implemented
        mockAdapterInstance.onPost(`${axiosInstance.defaults.baseURL}/api/UpdatePerformanceLibrary`)
            .reply(200, updatePerformanceLibraryData);
        return axiosInstance.post<PerformanceLibrary>('/api/UpdatePerformanceLibrary', updatePerformanceLibraryData);
    }

    /**
     * Gets a scenario's performance library
     * @param selectedScenarioId Scenario object id
     */
    static getScenarioPerformanceLibrary(selectedScenarioId: number): AxiosPromise<PerformanceLibrary> {
        // TODO: remove mockAdapter code when api is implemented
        mockAdapterInstance.onGet(`${axiosInstance.defaults.baseURL}/api/GetScenarioPerformanceLibrary/${selectedScenarioId}`)
            .reply(200, mockScenarioPerformanceLibrary);
        return axiosInstance.get<PerformanceLibrary>(`/api/GetScenarioPerformanceLibrary/${selectedScenarioId}`);
    }

    /**
     * Upserts a scenario performance library
     * @param saveScenarioPerformanceLibraryData The scenario performance library upsert data
     */
    static saveScenarioPerformanceLibrary(saveScenarioPerformanceLibraryData: PerformanceLibrary): AxiosPromise<PerformanceLibrary> {
        // TODO: remove mockAdapter code when api is implemented
        mockAdapterInstance.onPost(`${axiosInstance.defaults.baseURL}/api/SaveScenarioPerformanceLibrary`)
            .reply(200, saveScenarioPerformanceLibraryData);
        return axiosInstance.post<PerformanceLibrary>('/api/SaveScenarioPerformanceLibrary', saveScenarioPerformanceLibraryData);
    }
}
