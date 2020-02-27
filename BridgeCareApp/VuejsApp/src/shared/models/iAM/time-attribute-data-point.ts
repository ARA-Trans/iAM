export interface TimeAttributeDataPoint {
    id: string;
    timeValue: number;
    attributeValue: number;
}

export const emptyTimeAttributeDataPoint = {
    id: '',
    timeValue: 0,
    attributeValue: 0
};
