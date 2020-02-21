export interface Target {
    id: string;
    attribute: string;
    name: string;
    year: number;
    targetMean: number;
    criteria: string;
}

export interface TargetLibrary {
    id: string;
    name: string;
    owner?: string;
    description: string;
    targets: Target[];
}

export const emptyTarget: Target = {
    id: '0',
    attribute: '',
    name: '',
    year: 0,
    targetMean: 1,
    criteria: ''
};

export const emptyTargetLibrary: TargetLibrary = {
    id: '0',
    name: '',
    description: '',
    targets: []
};
