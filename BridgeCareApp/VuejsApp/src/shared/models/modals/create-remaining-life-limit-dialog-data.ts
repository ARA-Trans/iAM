import {SelectItem} from '@/shared/models/vue/select-item';

export interface CreateRemainingLifeLimitDialogData {
    showDialog: boolean;
    numericAttributesSelectListItems: SelectItem[];
}

export const emptyCreateRemainingLifeLimitDialogData: CreateRemainingLifeLimitDialogData = {
    showDialog: false,
    numericAttributesSelectListItems: []
};
