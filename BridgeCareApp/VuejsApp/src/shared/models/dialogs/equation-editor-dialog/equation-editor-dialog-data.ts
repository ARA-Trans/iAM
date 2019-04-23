import {PerformanceStrategyEquation} from '@/shared/models/iAM/performance';

export interface EquationEditorDialogData {
    showDialog: boolean;
    equation: string;
    isPiecewise: boolean;
    isFunction: boolean;
}

export const emptyEquationEditorDialogData: EquationEditorDialogData = {
    showDialog: false,
    equation: '',
    isPiecewise: false,
    isFunction: false
};
