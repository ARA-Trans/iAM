export interface PerformanceLibraryEquation {
    id: string;
    attribute: string;
    equationName: string;
    equation: string;
    criteria: string;
    shift: boolean;
    piecewise: boolean;
    isFunction: boolean;
}

export interface PerformanceLibrary {
    id: string;
    name: string;
    description: string;
    equations: PerformanceLibraryEquation[];
}

export const emptyEquation: PerformanceLibraryEquation = {
    id: '0',
    attribute: '',
    equationName: '',
    equation: '',
    criteria: '',
    shift: false,
    piecewise: false,
    isFunction: false,
};

export const emptyPerformanceLibrary: PerformanceLibrary = {
    id: '0',
    name: '',
    description: '',
    equations: []
};
