import {RemainingLifeLimit} from '@/shared/models/iAM/remaining-life-limit';

export interface CreateRemainingLifeLimitLibraryDialogData {
    showDialog: boolean;
    description: string;
    remainingLifeLimits: RemainingLifeLimit[];
}

export const emptyCreateRemainingLifeLimitLibraryDialogData: CreateRemainingLifeLimitLibraryDialogData = {
    showDialog: false,
    description: '',
    remainingLifeLimits: []
};
