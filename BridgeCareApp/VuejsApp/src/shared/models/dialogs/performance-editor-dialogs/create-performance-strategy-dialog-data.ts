import {PerformanceStrategyEquation} from '@/shared/models/iAM/performance';

export interface CreatePerformanceStrategyDialogData {
    showDialog: boolean;
    selectedPerformanceStrategyDescription: string;
    selectedPerformanceStrategyEquations: PerformanceStrategyEquation[];
}

export const emptyCreatePerformanceStrategyDialogData: CreatePerformanceStrategyDialogData = {
    showDialog: false,
    selectedPerformanceStrategyDescription: '',
    selectedPerformanceStrategyEquations: []
};
