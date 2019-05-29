import {PerformanceLibraryEquation} from '@/shared/models/iAM/performance';

export interface EquationEditorDialogData {
    showDialog: boolean;
    equation: string;
    canBePiecewise: boolean;
    isPiecewise: boolean;
    isFunction: boolean;
}

export const emptyEquationEditorDialogData: EquationEditorDialogData = {
    showDialog: false,
    equation: '',
    canBePiecewise: false,
    isPiecewise: false,
    isFunction: false
};
