import {AxiosPromise} from 'axios';
import {TreatmentLibrary} from '@/shared/models/iAM/treatment';
import {axiosInstance} from '@/shared/utils/axios-instance';
// TODO: remove MockAdapter code when api is implemented
import MockAdapter from 'axios-mock-adapter';
import {mockScenarioTreatmentLibrary, mockTreatmentLibraries} from '@/shared/utils/mock-data';

export default class TreatmentEditorService {

    /**
     * Gets all treatment libraries
     */
    static getTreatmentLibraries(): AxiosPromise<TreatmentLibrary[]> {
        // TODO: remove MockAdapter code when api is implemented
        const mockAdapterInstance = new MockAdapter(axiosInstance)
            .onGet(`${axiosInstance.defaults.baseURL}/api/GetTreatmentLibraries`)
            .reply((config: any) => {
                mockAdapterInstance.restore();
                return [200, mockTreatmentLibraries];
            });
        return axiosInstance.get<TreatmentLibrary[]>('/api/GetTreatmentLibraries');
    }

    /**
     * Creates a treatment library
     * @param createTreatmentLibraryData The treatment library create data
     */
    static createTreatmentLibrary(createTreatmentLibraryData: TreatmentLibrary): AxiosPromise<TreatmentLibrary> {
        // TODO: remove MockAdapter code when api is implemented
        const mockAdapterInstance = new MockAdapter(axiosInstance)
            .onPost(`${axiosInstance.defaults.baseURL}/api/CreateTreatmentLibrary`)
            .reply((config: any) => {
                mockAdapterInstance.restore();
                return [200, createTreatmentLibraryData];
            });
        return axiosInstance.post<TreatmentLibrary>('/api/CreateTreatmentLibrary', createTreatmentLibraryData);
    }

    /**
     * Updates a treatment library
     * @param updateTreatmentLibraryData The treatment library update data
     */
    static updateTreatmentLibrary(updateTreatmentLibraryData: TreatmentLibrary): AxiosPromise<TreatmentLibrary> {
        // TODO: remove MockAdapter code when api is implemented
        const mockAdapterInstance = new MockAdapter(axiosInstance)
            .onPost(`${axiosInstance.defaults.baseURL}/api/UpdateTreatmentLibrary`)
            .reply((config: any) => {
                mockAdapterInstance.restore();
                return [200, updateTreatmentLibraryData];
            });
        return axiosInstance.post<TreatmentLibrary>('/api/UpdateTreatmentLibrary', updateTreatmentLibraryData);
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