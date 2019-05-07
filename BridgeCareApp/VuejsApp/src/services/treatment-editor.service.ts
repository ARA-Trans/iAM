import axios from 'axios';
import {TreatmentLibrary} from '@/shared/models/iAM/treatment';
import {mockScenarioTreatmentLibrary, mockTreatmentLibraries} from '@/shared/utils/mock-data';

axios.defaults.baseURL = process.env.VUE_APP_URL;

export default class TreatmentEditorService {
    /**
     * Gets all treatment libraries a user can read/edit
     */
    getTreatmentLibraries(): Promise<TreatmentLibrary[]> {
        return Promise.resolve<TreatmentLibrary[]>(mockTreatmentLibraries);
        // TODO: add axios web service call for treatment libraries
    }

    /**
     * Creates a treatment library
     * @param createdTreatmentLibrary The treatment library create data
     */
    createTreatmentLibrary(createdTreatmentLibrary: TreatmentLibrary): Promise<any> {
        return Promise.resolve<any>({} as any);
        // TODO: add axios web service call for creating treatment library
    }

    /**
     * Updates a treatment library
     * @param updatedTreatmentLibrary The treatment library update data
     */
    updateTreatmentLibrary(updatedTreatmentLibrary: TreatmentLibrary): Promise<any> {
        return Promise.resolve<any>({} as any);
        // TODO: add axios web service call for updating treatment library
    }

    /**
     * Gets a scenario's treatment library
     * @param scenarioId The id of the scenario to use to get the scenario treatment library
     */
    getScenarioTreatmentLibrary(scenarioId: number): Promise<TreatmentLibrary> {
        return Promise.resolve<TreatmentLibrary>(mockScenarioTreatmentLibrary);
        // TODO: add axios web service call for scenario treatment library
    }

    /**
     * Upserts a scenario treatment library
     * @param upsertedScenarioTreatmentLibrary The scenario treatment library upsert data
     */
    upsertScenarioTreatmentLibrary(upsertedScenarioTreatmentLibrary: TreatmentLibrary): Promise<any> {
        return Promise.resolve<any>({} as any);
        // TODO: add axios web service call for upserting scenario treatment library
    }
}