import axios from 'axios';
import {PerformanceStrategy} from '@/shared/models/iAM/performance';
import {mockPerformanceStrategies} from '@/shared/utils/mock-data';

axios.defaults.baseURL = process.env.VUE_APP_URL;

export default class PerformanceEditorService {
    /**
     * Gets all performance strategies a user can read/edit
     */
    getPerformanceStrategies(): Promise<PerformanceStrategy[]> {
        return Promise.resolve<PerformanceStrategy[]>(mockPerformanceStrategies);
        // TODO: add axios web service call for performance strategies
    }

    /**
     * Creates a performance strategy
     * @param createdPerformanceStrategy The performance strategy create data
     */
    createPerformanceStrategy(createdPerformanceStrategy: PerformanceStrategy): Promise<PerformanceStrategy> {
        return Promise.resolve<PerformanceStrategy>(createdPerformanceStrategy);
        // TODO: add axios web service call for creating performance strategy
    }

    /**
     * Updates a performance strategy
     * @param updatePerformanceStrategy The performance strategy update data
     */
    updatePerformanceStrategy(updatedPerformanceStrategy: PerformanceStrategy): Promise<PerformanceStrategy> {
        return Promise.resolve<PerformanceStrategy>(updatedPerformanceStrategy);
        // TODO: add axios web service call for updating performance strategy
    }
}
