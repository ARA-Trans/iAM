import {AxiosPromise} from 'axios';
import {Consequence, Cost, Treatment, TreatmentLibrary} from '@/shared/models/iAM/treatment';
import {axiosInstance, nodejsAxiosInstance} from '@/shared/utils/axios-instance';

const modifyDataForMongoDB = (treatmentLibrary: TreatmentLibrary): any => {
    const treatmentLibraryData: any = {
        ...treatmentLibrary,
        _id: treatmentLibrary.id,
        treatments: treatmentLibrary.treatments.map((treatment: Treatment) => {
            const treatmentData: any = {
                ...treatment,
                _id: treatment.id,
                feasibility: {
                    ...treatment.feasibility,
                    _id: treatment.feasibility.id
                },
                costs: treatment.costs.map((cost: Cost) => {
                    const costData: any = {
                        ...cost,
                        _id: cost.id
                    };
                    delete costData.id;
                    return costData;
                }),
                consequences: treatment.consequences.map((consequence: Consequence) => {
                    const consequenceData: any = {
                        ...consequence,
                        _id: consequence.id
                    };
                    delete consequenceData.id;
                    return consequenceData;
                })
            };
            delete treatmentData.id;
            delete treatmentData.feasibility.id;
            return treatmentData as Treatment;
        })
    };
    delete treatmentLibraryData.id;
    return treatmentLibraryData;
};

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
        return nodejsAxiosInstance.post('/api/CreateTreatmentLibrary', modifyDataForMongoDB(createTreatmentLibraryData));
    }

    /**
     * Updates a treatment library
     * @param updateTreatmentLibraryData The treatment library update data
     */
    static updateTreatmentLibrary(updateTreatmentLibraryData: TreatmentLibrary): AxiosPromise {
        return nodejsAxiosInstance.put('/api/UpdateTreatmentLibrary', modifyDataForMongoDB(updateTreatmentLibraryData));
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