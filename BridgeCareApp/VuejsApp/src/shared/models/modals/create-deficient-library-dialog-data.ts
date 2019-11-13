import {Deficient} from '@/shared/models/iAM/deficient';

export interface CreateDeficientLibraryDialogData {
    showDialog: boolean;
    description: string;
    deficients: Deficient[];
}

export const emptyCreateDeficientLibraryDialogData: CreateDeficientLibraryDialogData = {
    showDialog: false,
    description: '',
    deficients: []
};
