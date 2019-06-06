export interface Deficient {
    scenarioId: number;
    id: number;
    attribute: string;
    name: string;
    deficient: number;
    percentDeficient: number;
    criteria: string;
}

export const emptyDeficient: Deficient = {
    scenarioId: 0,
    id: 0,
    attribute: '',
    name: '',
    deficient: 0,
    percentDeficient: 0,
    criteria: ''
};
