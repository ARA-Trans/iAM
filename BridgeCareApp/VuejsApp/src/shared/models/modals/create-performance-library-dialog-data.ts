import {PerformanceLibraryEquation} from '@/shared/models/iAM/performance';

export interface CreatePerformanceLibraryDialogData {
    showDialog: boolean;
    description: string;
    equations: PerformanceLibraryEquation[];
}

export const emptyCreatePerformanceLibraryDialogData: CreatePerformanceLibraryDialogData = {
    showDialog: false,
    description: '',
    equations: []
};
