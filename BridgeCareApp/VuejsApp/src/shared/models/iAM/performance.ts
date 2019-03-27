export interface Equation {
    attribute: string;
    equationName: string;
    equation: string;
    criteria: string;
    shift: boolean;
    piecewise: boolean;
    isFunction: boolean;
}

export interface PerformanceStrategyEquation extends Equation{
    performanceStrategyId: number;
    performanceStrategyEquationId: number;
}

export interface ScenarioEquation extends Equation {
    scenarioId: number;
    scenarioEquationId: number;
}

export interface CreatedPerformanceStrategyEquation {
    performanceStrategyId: number;
    attribute: string;
    equationName: string;
}

export interface DeletedPerformanceStrategyEquations {
    performanceStrategyId: number;
    deletedEquationIds: number[];
}

export interface PerformanceStrategy {
    id: number;
    name: string;
    description: string;
    performanceStrategyEquations: PerformanceStrategyEquation[];
}

export interface CreatedPerformanceStrategy {
    name: string;
    description: string;
    performanceStrategyEquations: PerformanceStrategyEquation[];
}

export interface UpdatedPerformanceStrategy extends PerformanceStrategy{
    deletedPerformanceStrategyEquations: number[];
}

export const emptyPerformanceStrategy: PerformanceStrategy = {
    id: 0,
    name: '',
    description: '',
    performanceStrategyEquations: []
};

export const emptyCreatedPerformanceStrategy: CreatedPerformanceStrategy = {
    name: '',
    description: '',
    performanceStrategyEquations: []
};

export const emptyPerformanceStrategyEquation: PerformanceStrategyEquation = {
    performanceStrategyId: 0,
    performanceStrategyEquationId: 0,
    attribute: '',
    equationName: '',
    equation: '',
    criteria: '',
    shift: false,
    piecewise: false,
    isFunction: false
};
