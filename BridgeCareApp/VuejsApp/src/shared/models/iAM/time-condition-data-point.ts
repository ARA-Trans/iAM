export interface TimeConditionDataPoint {
    id: string;
    timeValue: number;
    conditionValue: number;
}

export const emptyTimeConditionDataPoint = {
    id: '',
    timeValue: 0,
    conditionValue: 0
};
