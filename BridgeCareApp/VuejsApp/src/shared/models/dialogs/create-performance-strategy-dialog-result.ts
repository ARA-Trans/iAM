import {PerformanceStrategy} from '@/shared/models/iAM/performance';

export interface CreatePerformanceStrategyDialogResult {
    canceled: boolean;
    newPerformanceStrategy: PerformanceStrategy;
}
