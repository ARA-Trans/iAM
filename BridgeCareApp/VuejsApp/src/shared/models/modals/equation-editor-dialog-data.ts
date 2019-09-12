export interface EquationEditorDialogData {
    showDialog: boolean;
    equation: string;
    canBePiecewise: boolean;
    isPiecewise: boolean;
}

export const emptyEquationEditorDialogData: EquationEditorDialogData = {
    showDialog: false,
    equation: '',
    canBePiecewise: false,
    isPiecewise: false
};
