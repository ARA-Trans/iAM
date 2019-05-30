import {AxiosPromise} from 'axios';
import {TreatmentLibrary} from '@/shared/models/iAM/treatment';
import {axiosInstance} from '@/shared/utils/axios-instance';
// TODO: remove MockAdapter code when api is implemented
import {mockTreatmentLibraries} from '@/shared/utils/mock-data';
import {mockAdapter, mockAxiosInstance} from '@/shared/utils/axios-mock-adapter-instance';

export default class TreatmentEditorService {

    /**
     * Gets all treatment libraries
     */
    static getTreatmentLibraries(): AxiosPromise<TreatmentLibrary[]> {
        // TODO: remove MockAdapter code when api is implemented
        mockAdapter
            .onGet('/api/GetTreatmentLibraries')
            .reply((config: any) => {
                return [200, mockTreatmentLibraries];
            });
        // TODO: replace mockAxiosInstance with axiosInstance
        return mockAxiosInstance.get<TreatmentLibrary[]>('/api/GetTreatmentLibraries');
    }

    /**
     * Creates a treatment library
     * @param createTreatmentLibraryData The treatment library create data
     */
    static createTreatmentLibrary(createTreatmentLibraryData: TreatmentLibrary): AxiosPromise<TreatmentLibrary> {
        // TODO: remove MockAdapter code when api is implemented
        mockAdapter
            .onPost('/api/CreateTreatmentLibrary')
            .reply((config: any) => {
                return [200, createTreatmentLibraryData];
            });
        // TODO: replace mockAxiosInstance with axiosInstance
        return mockAxiosInstance.post<TreatmentLibrary>('/api/CreateTreatmentLibrary', createTreatmentLibraryData);
    }

    /**
     * Updates a treatment library
     * @param updateTreatmentLibraryData The treatment library update data
     */
    static updateTreatmentLibrary(updateTreatmentLibraryData: TreatmentLibrary): AxiosPromise<TreatmentLibrary> {
        // TODO: remove MockAdapter code when api is implemented
        mockAdapter
            .onPost('/api/UpdateTreatmentLibrary')
            .reply((config: any) => {
                return [200, updateTreatmentLibraryData];
            });
        // TODO: replace mockAxiosInstance with axiosInstance
        return mockAxiosInstance.post<TreatmentLibrary>('/api/UpdateTreatmentLibrary', updateTreatmentLibraryData);
    }

    /**
     * Gets a scenario's treatment library data
     * @param selectedScenarioId Scenario object id
     */
    static getScenarioTreatmentLibrary(selectedScenarioId: number): AxiosPromise<TreatmentLibrary> {
        return axiosInstance.get<TreatmentLibrary>(`/api/GetScenarioTreatmentLibrary/${selectedScenarioId}`);
    }

    /**
     * Saves a scenario's treatment library data
     * @param saveScenarioTreatmentLibraryData The scenario treatment library save data
     */
    static saveScenarioTreatmentLibrary(saveScenarioTreatmentLibraryData: TreatmentLibrary): AxiosPromise<TreatmentLibrary> {
        return axiosInstance.post<TreatmentLibrary>('/api/SaveScenarioTreatmentLibrary', saveScenarioTreatmentLibraryData);
    }
}