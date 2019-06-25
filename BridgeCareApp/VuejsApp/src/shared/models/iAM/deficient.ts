export interface Deficient {
    scenarioId: number;
    id: string;
    attribute: string;
    name: string;
    deficient: number;
    percentDeficient: number;
    criteria: string;
}

export const emptyDeficient: Deficient = {
    scenarioId: 0,
    id: '0',
    attribute: '',
    name: '',
    deficient: 1,
    percentDeficient: 1,
    criteria: ''
};
