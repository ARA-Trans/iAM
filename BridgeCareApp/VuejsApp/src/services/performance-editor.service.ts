import {AxiosPromise} from 'axios';
import {PerformanceLibrary} from '@/shared/models/iAM/performance';
import {axiosInstance} from '@/shared/utils/axios-instance';
// TODO: remove MockAdapter code when api is implemented
import {mockPerformanceLibraries} from '@/shared/utils/mock-data';
import {mockAdapter, mockAxiosInstance} from '@/shared/utils/axios-mock-adapter-instance';

export default class PerformanceEditorService {
    /**
     * Gets all performance Libraries a user can read/edit
     */
    static getPerformanceLibraries(): AxiosPromise<PerformanceLibrary[]> {
        // TODO: remove MockAdapter code when api is implemented
        mockAdapter
            .onGet('/api/GetPerformanceLibraries')
            .reply((config: any) => {
                return [200, mockPerformanceLibraries];
            });
        // TODO: replace mockAxiosInstance with axiosInstance
        return mockAxiosInstance.get<PerformanceLibrary[]>('/api/GetPerformanceLibraries');
    }

    /**
     * Creates a performance library
     * @param createPerformanceLibraryData The performance library create data
     */
    static createPerformanceLibrary(createPerformanceLibraryData: PerformanceLibrary): AxiosPromise<PerformanceLibrary> {
        // TODO: remove MockAdapter code when api is implemented
        mockAdapter
            .onPost('/api/CreatePerformanceLibrary')
            .reply((config: any) => {
                return [200, createPerformanceLibraryData];
            });
        // TODO: replace mockAxiosInstance with axiosInstance
        return mockAxiosInstance.post<PerformanceLibrary>('/api/CreatePerformanceLibrary', createPerformanceLibraryData);
    }

    /**
     * Updates a performance library
     * @param updatePerformanceLibraryData The performance library update data
     */
    static updatePerformanceLibrary(updatePerformanceLibraryData: PerformanceLibrary): AxiosPromise<PerformanceLibrary> {
        // TODO: remove MockAdapter code when api is implemented
        mockAdapter
            .onPost('/api/UpdatePerformanceLibrary')
            .reply((config: any) => {
                return [200, updatePerformanceLibraryData];
            });
        // TODO: replace mockAxiosInstance with axiosInstance
        return mockAxiosInstance.post<PerformanceLibrary>('/api/UpdatePerformanceLibrary', updatePerformanceLibraryData);
    }

    /**
     * Gets a scenario's performance library
     * @param selectedScenarioId Scenario object id
     */
    static getScenarioPerformanceLibrary(selectedScenarioId: number): AxiosPromise<PerformanceLibrary> {
        return axiosInstance.get<PerformanceLibrary>(`/api/GetScenarioPerformanceLibrary/${selectedScenarioId}`);
    }

    /**
     * Saves a scenario performance library
     * @param saveScenarioPerformanceLibraryData The scenario performance library upsert data
     */
    static saveScenarioPerformanceLibrary(saveScenarioPerformanceLibraryData: PerformanceLibrary): AxiosPromise<PerformanceLibrary> {
        return axiosInstance.post<PerformanceLibrary>('/api/SaveScenarioPerformanceLibrary', saveScenarioPerformanceLibraryData);
    }
}
