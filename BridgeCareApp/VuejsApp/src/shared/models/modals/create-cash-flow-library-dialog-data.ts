import {CashFlowDuration, CashFlowParameter} from '@/shared/models/iAM/cash-flow';

export interface CreateCashFlowLibraryDialogData {
    showDialog: boolean;
    parameters: CashFlowParameter[];
    durations: CashFlowDuration[];
}

export const emptyCreateCashFlowLibraryDialogData = {
    showDialog: false,
    parameters: [],
    durations: []
};