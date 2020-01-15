import {SplitTreatment} from '@/shared/models/iAM/cash-flow';

export interface CreateCashFlowLibraryDialogData {
    showDialog: boolean;
    splitTreatments: SplitTreatment[];
}

export const emptyCreateCashFlowLibraryDialogData = {
    showDialog: false,
    splitTreatments: []
};