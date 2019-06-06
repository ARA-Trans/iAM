export interface Target {
    scenarioId: number;
    id: number;
    attribute: string;
    name: string;
    year: number;
    targetMean: number;
    criteria: string;
}

export const emptyTarget: Target = {
    scenarioId: 0,
    id: 0,
    attribute: '',
    name: '',
    year: 0,
    targetMean: 0,
    criteria: ''
};
