export interface Deficient {
    id: string;
    attribute: string;
    name: string;
    deficient: number;
    percentDeficient: number;
    criteria: string;
}

export interface DeficientLibrary {
    id: string;
    name: string;
    owner?: string;
    shared?: boolean;
    description: string;
    deficients: Deficient[];
}

export const emptyDeficient: Deficient = {
    id: '0',
    attribute: '',
    name: '',
    deficient: 0,
    percentDeficient: 0,
    criteria: ''
};

export const emptyDeficientLibrary: DeficientLibrary = {
    id: '0',
    name: '',
    description: '',
    deficients: []
};
