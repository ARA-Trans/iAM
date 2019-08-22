import {Priority} from '@/shared/models/iAM/priority';

export interface CreatePriorityLibraryDialogData {
    showDialog: boolean;
    description: string;
    priorities: Priority[];
}

export const emptyCreatePriorityLibraryDialogData: CreatePriorityLibraryDialogData = {
    showDialog: false,
    description: '',
    priorities: []
};
