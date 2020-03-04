export interface SelectItem {
    text: string;
    value: string | number;
}

export const emptySelectItem: SelectItem = {
    text: '',
    value: ''
};
