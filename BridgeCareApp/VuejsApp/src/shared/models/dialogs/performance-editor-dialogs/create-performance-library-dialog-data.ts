import {PerformanceLibraryEquation} from '@/shared/models/iAM/performance';

export interface CreatePerformanceLibraryDialogData {
    showDialog: boolean;
    selectedPerformanceLibraryDescription: string;
    selectedPerformanceLibraryEquations: PerformanceLibraryEquation[];
}

export const emptyCreatePerformanceLibraryDialogData: CreatePerformanceLibraryDialogData = {
    showDialog: false,
    selectedPerformanceLibraryDescription: '',
    selectedPerformanceLibraryEquations: []
};
