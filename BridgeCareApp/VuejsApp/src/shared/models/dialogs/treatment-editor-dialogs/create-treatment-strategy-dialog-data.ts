import {Treatment} from '@/shared/models/iAM/treatment';

export interface CreateTreatmentStrategyDialogData {
    showDialog: boolean;
    selectedTreatmentStrategyDescription: string;
    selectedTreatmentStrategyTreatments: Treatment[];
}

export const emptyCreateTreatmentStrategyDialogData: CreateTreatmentStrategyDialogData = {
    showDialog: false,
    selectedTreatmentStrategyDescription: '',
    selectedTreatmentStrategyTreatments: []
};
