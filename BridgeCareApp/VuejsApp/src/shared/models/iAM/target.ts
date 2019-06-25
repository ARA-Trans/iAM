export interface Target {
    scenarioId: number;
    id: string;
    attribute: string;
    name: string;
    year: number;
    targetMean: number;
    criteria: string;
}

export const emptyTarget: Target = {
    scenarioId: 0,
    id: '0',
    attribute: '',
    name: '',
    year: 0,
    targetMean: 1,
    criteria: ''
};
