import axios from 'axios';
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
     * @param scenarioId The id of the scenario to use to get the scenario performance library
     */
    getScenarioPerformanceLibrary(scenarioId: number): Promise<PerformanceLibrary> {
        return Promise.resolve<PerformanceLibrary>(mockScenarioPerformanceLibrary);
        // TODO: add axios web service call for scenario performance library
    }

    /**
     * Upserts a scenario performance library
     * @param upsertedScenarioPerformanceLibrary The scenario performance library upsert data
     */
    upsertScenarioPerformanceLibrary(upsertedScenarioPerformanceLibrary: PerformanceLibrary): Promise<PerformanceLibrary> {
        return Promise.resolve<any>(upsertedScenarioPerformanceLibrary);
        // TODO: add axios web service call for upserting scenario performance library
    }
}
