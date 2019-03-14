import {PerformanceEquation} from '@/shared/models/iAM/performance';

export interface CreatePerformanceEquationDialogResult {
    canceled: boolean;
    newPerformanceEquation: PerformanceEquation;
}
