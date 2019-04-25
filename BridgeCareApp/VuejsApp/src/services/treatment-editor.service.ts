import axios from 'axios';
import {TreatmentStrategy} from '@/shared/models/iAM/treatment';
import {mockTreatmentStrategies} from '@/shared/utils/mock-data';

axios.defaults.baseURL = process.env.VUE_APP_URL;

export default class TreatmentEditorService {
    /**
     * Gets all treatment strategies a user can read/edit
     */
    getTreatmentStrategies(): Promise<TreatmentStrategy[]> {
        return Promise.resolve<TreatmentStrategy[]>(mockTreatmentStrategies);
        // TODO: add axios web service call for treatment strategies
    }

    /**
     * Creates a treatment strategy
     * @param createdTreatmentStrategy The treatment strategy create data
     */
    createTreatmentStrategy(createdTreatmentStrategy: TreatmentStrategy): Promise<TreatmentStrategy> {
        return Promise.resolve<TreatmentStrategy>(createdTreatmentStrategy);
        // TODO: add axios web service call for creating treatment strategy
    }

    /**
     * Updates a treatment strategy
     * @param updatedTreatmentStrategy The treatment strategy update data
     */
    updateTreatmentStrategy(updatedTreatmentStrategy: TreatmentStrategy): Promise<TreatmentStrategy> {
        return Promise.resolve<TreatmentStrategy>(updatedTreatmentStrategy);
        // TODO: add axios web service call for updating treatment strategy
    }
}