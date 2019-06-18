export interface Feasibility {
    id: number | string;
    criteria: string;
    yearsBeforeAny: number;
    yearsBeforeSame: number;
}

export interface Cost {
    id: number | string;
    equation: string;
    isFunction: boolean;
    criteria: string;
}

export interface Consequence {
    id: number | string;
    attribute: string;
    change: string;
    equation: string;
    isFunction: boolean;
    criteria: string;
}

export interface Treatment {
    id: number | string;
    name: string;
    feasibility: Feasibility;
    costs: Cost[];
    consequences: Consequence[];
    budgets: string[];
}

export interface TreatmentLibrary {
    id: number | string;
    name: string;
    description: string;
    treatments: Treatment[];
}

export interface BudgetGridRow {
    budget: string;
}

export const emptyFeasibility: Feasibility = {
    id: 0,
    criteria: '',
    yearsBeforeAny: 0,
    yearsBeforeSame: 0,
};

export const emptyCost: Cost = {
    id: 0,
    equation: '',
    isFunction: false,
    criteria: ''
};

export const emptyConsequence: Consequence = {
    id: 0,
    attribute: '',
    change: '',
    equation: '',
    isFunction: false,
    criteria: ''
};

export const emptyTreatment: Treatment = {
    id: 0,
    name: '',
    feasibility: emptyFeasibility,
    costs: [],
    consequences: [],
    budgets: []
};

export const emptyTreatmentLibrary: TreatmentLibrary = {
    id: 0,
    name: '',
    description: '',
    treatments: []
};
