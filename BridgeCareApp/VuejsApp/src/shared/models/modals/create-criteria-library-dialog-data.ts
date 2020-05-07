export interface CreateCriteriaLibraryDialogData {
    showDialog: boolean;
    description: string;
    criteria: string;
}

export const emptyCreateCriteriaLibraryDialogData: CreateCriteriaLibraryDialogData = {
    showDialog: false,
    description: '',
    criteria: ''
};
