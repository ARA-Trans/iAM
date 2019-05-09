export interface PerformanceLibraryEquation {
    performanceLibraryId: number;
    id: number;
    attribute: string;
    equationName: string;
    equation: string;
    criteria: string;
    shift: boolean;
    piecewise: boolean;
    isFunction: boolean;
}

export interface PerformanceLibrary {
    id: number;
    name: string;
    description: string;
    equations: PerformanceLibraryEquation[];
}

export const emptyEquation: PerformanceLibraryEquation = {
    performanceLibraryId: 0,
    id: 0,
    attribute: '',
    equationName: '',
    equation: '',
    criteria: '',
    shift: false,
    piecewise: false,
    isFunction: false,
};

export const emptyPerformanceLibrary: PerformanceLibrary = {
    id: 0,
    name: '',
    description: '',
    equations: []
};
