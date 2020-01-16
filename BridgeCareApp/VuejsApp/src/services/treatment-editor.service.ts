import {AxiosPromise} from 'axios';
import {TreatmentLibrary} from '@/shared/models/iAM/treatment';
import {axiosInstance, nodejsAxiosInstance} from '@/shared/utils/axios-instance';
import {convertFromVueToMongo} from '@/shared/utils/mongo-model-conversion-utils';

export default class TreatmentEditorService {
    /**
     * Gets all treatment libraries
     */
    static getTreatmentLibraries(): AxiosPromise {
        return nodejsAxiosInstance.get('/api/GetTreatmentLibraries');
    }

    /**
     * Creates a treatment library
     * @param createTreatmentLibraryData The treatment library create data
     */
    static createTreatmentLibrary(createTreatmentLibraryData: TreatmentLibrary): AxiosPromise {
        return nodejsAxiosInstance.post('/api/CreateTreatmentLibrary', convertFromVueToMongo(createTreatmentLibraryData));
    }

    /**
     * Updates a treatment library
     * @param updateTreatmentLibraryData The treatment library update data
     */
    static updateTreatmentLibrary(updateTreatmentLibraryData: TreatmentLibrary): AxiosPromise {
        return nodejsAxiosInstance.put('/api/UpdateTreatmentLibrary', convertFromVueToMongo(updateTreatmentLibraryData));
    }

    /**
     * Deletes a treatment library
     * @param treatmentLibraryData The treatment library to delete
     */
    static deleteTreatmentLibrary(treatmentLibraryData: TreatmentLibrary): AxiosPromise {
        return nodejsAxiosInstance.delete(`/api/DeleteTreatmentLibrary/${treatmentLibraryData.id}`);
    }

    /**
     * Gets a scenario's treatment library data
     * @param selectedScenarioId Scenario object id
     */
    static getScenarioTreatmentLibrary(selectedScenarioId: number): AxiosPromise {
        return axiosInstance.get(`/api/GetScenarioTreatmentLibrary/${selectedScenarioId}`);
    }

    /**
     * Saves a scenario's treatment library data
     * @param saveScenarioTreatmentLibraryData The scenario treatment library save data
     */
    static saveScenarioTreatmentLibrary(saveScenarioTreatmentLibraryData: TreatmentLibrary): AxiosPromise {
        return axiosInstance.post('/api/SaveScenarioTreatmentLibrary', saveScenarioTreatmentLibraryData);
    }
}