export interface PerformanceEquation {
    performanceId: number;
    attribute: string;
    equationName: string;
    equation: string;
    criteria: string;
    shift: boolean;
    piecewise: boolean;
    isFunction: boolean;
}

export interface PerformanceStrategy {
    networkId: number;
    simulationId: number;
    performanceEquations: PerformanceEquation[];
}

export interface SavedPerformanceStrategy extends PerformanceStrategy{
    deletedPerformanceEquations: number[];
}

export const emptyPerformanceStrategy: PerformanceStrategy = {
    networkId: 0,
    simulationId: 0,
    performanceEquations: []
};

export const emptyPerformanceEquation: PerformanceEquation = {
    performanceId: 0,
    attribute: '',
    equationName: '',
    equation: '',
    criteria: '',
    shift: false,
    piecewise: false,
    isFunction: false
};
