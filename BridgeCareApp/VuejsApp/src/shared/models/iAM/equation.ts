export interface Equation {
    equation: string;
    isFunction: boolean;
    isPiecewise: boolean;
}

export interface EquationValidationResult {
    isValid: boolean;
    message: string;
}
