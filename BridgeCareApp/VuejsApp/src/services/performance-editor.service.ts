import Vue from 'vue';
import axios, { AxiosResponse } from 'axios';
import { isNil } from 'ramda';
import {PerformanceLibrary} from '@/shared/models/iAM/performance';
import {mockPerformanceLibraries, mockScenarioPerformanceLibrary} from '@/shared/utils/mock-data';

axios.defaults.baseURL = process.env.VUE_APP_URL;

export default class PerformanceEditorService {
    /**
     * Gets all performance Libraries a user can read/edit
     */
    getPerformanceLibraries(): Promise<PerformanceLibrary[]> {
        return Promise.resolve<PerformanceLibrary[]>(mockPerformanceLibraries);
        // TODO: add axios web service call for performance Libraries
    }

    /**
     * Creates a performance library
     * @param createdPerformanceLibrary The performance library create data
     */
    createPerformanceLibrary(createdPerformanceLibrary: PerformanceLibrary): Promise<PerformanceLibrary> {
        return Promise.resolve<any>(createdPerformanceLibrary);
        // TODO: add axios web service call for creating performance library
    }

    /**
     * Updates a performance library
     * @param updatePerformanceLibrary The performance library update data
     */
    updatePerformanceLibrary(updatedPerformanceLibrary: PerformanceLibrary): Promise<PerformanceLibrary> {
        return Promise.resolve<any>(updatedPerformanceLibrary);
        // TODO: add axios web service call for updating performance library
    }

    /**
     * Gets a scenario's performance library
     * @param selectedScenarioId The id of the scenario to use to get the scenario performance library
     */
    getScenarioPerformanceLibrary(selectedScenarioId: number): Promise<PerformanceLibrary> {
        return axios.get<PerformanceLibrary>(`/api/GetScenarioPerformanceLibrary/${selectedScenarioId}`)
            .then((response: AxiosResponse) => {
                if (!isNil(response)) {
                    return response.data;
                }
                return Promise.reject('Failed to get scenario performance library');
            });
    }

    /**
     * Upserts a scenario performance library
     * @param upsertedScenarioPerformanceLibrary The scenario performance library upsert data
     */
    upsertScenarioPerformanceLibrary(upsertedScenarioPerformanceLibrary: PerformanceLibrary): Promise<PerformanceLibrary> {
        return axios.post<PerformanceLibrary>('/api/SaveScenarioPerformanceLibrary', upsertedScenarioPerformanceLibrary)
            .then((response: AxiosResponse) => {
                if (!isNil(response)) {
                    return response.data;
                }
                return Promise.reject('Failed to apply performance library');
            });       
    }
}
