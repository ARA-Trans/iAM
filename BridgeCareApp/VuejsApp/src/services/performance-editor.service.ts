import axios from 'axios';
import {PerformanceStrategy, SavedPerformanceStrategy} from '@/shared/models/iAM/performance';

axios.defaults.baseURL = process.env.VUE_APP_URL;

export default class PerformanceEditorService {
    /**
     * Gets all performance strategies a user can read/edit
     */
    getPerformanceStrategies(): Promise<PerformanceStrategy[]> {
        return Promise.resolve<PerformanceStrategy[]>([]);
        // TODO: add axios web service call for performance strategies
    }

    /**
     * Creates/updates a performance strategy
     * @param savedPerformanceStrategy
     */
    savePerformanceStrategy(savedPerformanceStrategy: SavedPerformanceStrategy): Promise<PerformanceStrategy> {
        return Promise.resolve<PerformanceStrategy>({
           networkId: savedPerformanceStrategy.networkId,
           simulationId: savedPerformanceStrategy.simulationId,
           performanceEquations: savedPerformanceStrategy.performanceEquations
        });
        // TODO: add axios web service for saving performance strategy changes (create/update)
    }
}
