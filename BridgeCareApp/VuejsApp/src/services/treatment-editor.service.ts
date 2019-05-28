import axios, { AxiosResponse } from 'axios';
import { isNil } from 'ramda';
import {TreatmentLibrary} from '@/shared/models/iAM/treatment';
import {mockScenarioTreatmentLibrary, mockTreatmentLibraries} from '@/shared/utils/mock-data';

axios.defaults.baseURL = process.env.VUE_APP_URL;

export default class TreatmentEditorService {
    /**
     * Gets all treatment libraries
     */
    getTreatmentLibraries(): Promise<TreatmentLibrary[]> {
        return Promise.resolve<TreatmentLibrary[]>(mockTreatmentLibraries);
        // TODO: add axios web service call for treatment libraries
    }

    /**
     * Creates a treatment library
     * @param createdTreatmentLibrary The treatment library create data
     */
    createTreatmentLibrary(createdTreatmentLibrary: TreatmentLibrary): Promise<TreatmentLibrary> {
        return Promise.resolve<any>(createdTreatmentLibrary);
        // TODO: add axios web service call for creating treatment library
    }

    /**
     * Updates a treatment library
     * @param updatedTreatmentLibrary The treatment library update data
     */
    updateTreatmentLibrary(updatedTreatmentLibrary: TreatmentLibrary): Promise<TreatmentLibrary> {
        return Promise.resolve<any>(updatedTreatmentLibrary);
        // TODO: add axios web service call for updating treatment library
    }

    /**
     * Gets a scenario's treatment library data
     * @param selectedScenarioId to use in finding a scenario's treatment library data
     */
    getScenarioTreatmentLibrary(selectedScenarioId: number): Promise<TreatmentLibrary> {        
        return axios.get<TreatmentLibrary>(`/api/GetScenarioTreatmentLibrary/${selectedScenarioId}`)
            .then((response: AxiosResponse) => {
                if (!isNil(response)) {
                    return response.data;
                }
                return Promise.reject('Failed to get scenario treatment library');
            });
    }

    /**
     * Upserts a scenario's treatment library data
     * @param upsertedScenarioTreatmentLibrary The scenario treatment library upsert data
     */
    upsertScenarioTreatmentLibrary(upsertedScenarioTreatmentLibrary: TreatmentLibrary): Promise<TreatmentLibrary> {
        return axios.post<TreatmentLibrary>('/api/SaveScenarioTreatmentLibrary', upsertedScenarioTreatmentLibrary)
            .then((response: AxiosResponse) => {
                if (!isNil(response)) {
                    return response.data;
                }
                return Promise.reject('Failed to apply treatment library');
            });
    }
}