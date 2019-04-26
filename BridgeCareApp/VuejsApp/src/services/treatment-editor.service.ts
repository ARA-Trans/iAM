import axios from 'axios';
import {TreatmentLibrary} from '@/shared/models/iAM/treatment';
import {mockTreatmentLibraries} from '@/shared/utils/mock-data';

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
    createTreatmentLibrary(createdTreatmentLibrary: TreatmentLibrary): Promise<TreatmentLibrary> {
        return Promise.resolve<TreatmentLibrary>(createdTreatmentLibrary);
        // TODO: add axios web service call for creating treatment library
    }

    /**
     * Updates a treatment library
     * @param updatedTreatmentLibrary The treatment library update data
     */
    updateTreatmentLibrary(updatedTreatmentLibrary: TreatmentLibrary): Promise<TreatmentLibrary> {
        return Promise.resolve<TreatmentLibrary>(updatedTreatmentLibrary);
        // TODO: add axios web service call for updating treatment library
    }
}