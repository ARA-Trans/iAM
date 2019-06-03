import {Treatment} from '@/shared/models/iAM/treatment';

export interface CreateTreatmentLibraryDialogData {
    showDialog: boolean;
    selectedTreatmentLibraryDescription: string;
    selectedTreatmentLibraryTreatments: Treatment[];
}

export const emptyCreateTreatmentLibraryDialogData: CreateTreatmentLibraryDialogData = {
    showDialog: false,
    selectedTreatmentLibraryDescription: '',
    selectedTreatmentLibraryTreatments: []
};
