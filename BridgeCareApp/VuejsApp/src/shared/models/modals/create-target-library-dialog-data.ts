import {Target} from '@/shared/models/iAM/target';

export interface CreateTargetLibraryDialogData {
    showDialog: boolean;
    description: string;
    targets: Target[];
}

export const emptyCreateTargetLibraryDialogData: CreateTargetLibraryDialogData = {
    showDialog: false,
    description: '',
    targets: []
};
