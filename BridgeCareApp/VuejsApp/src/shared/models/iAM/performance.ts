export interface PerformanceStrategyEquation {
    performanceStrategyId: number;
    id: number;
    attribute: string;
    equationName: string;
    equation: string;
    criteria: string;
    shift: boolean;
    piecewise: boolean;
    isFunction: boolean;
}

export interface PerformanceStrategy {
    id: number;
    name: string;
    description: string;
    equations: PerformanceStrategyEquation[];
    deletedEquationIds: number[];
}

export const emptyEquation: PerformanceStrategyEquation = {
    performanceStrategyId: 0,
    id: 0,
    attribute: '',
    equationName: '',
    equation: '',
    criteria: '',
    shift: false,
    piecewise: false,
    isFunction: false,
};

export const emptyPerformanceStrategy: PerformanceStrategy = {
    id: 0,
    name: '',
    description: '',
    equations: [],
    deletedEquationIds: []
};
