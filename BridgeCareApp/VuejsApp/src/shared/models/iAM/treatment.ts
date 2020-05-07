export interface Feasibility {
    id: string;
    criteria: string;
    yearsBeforeAny: number;
    yearsBeforeSame: number;
}

export interface Cost {
    id: string;
    equation: string;
    isFunction?: boolean;
    criteria: string;
}

export interface Consequence {
    id: string;
    attribute: string;
    change: string;
    equation: string;
    isFunction?: boolean;
    criteria: string;
}

export interface Treatment {
    id: string;
    name: string;
    feasibility: Feasibility;
    costs: Cost[];
    consequences: Consequence[];
    budgets: string[];
}

export interface TreatmentLibrary {
    id: string;
    name: string;
    owner?: string;
    shared?: boolean;
    description: string;
    treatments: Treatment[];
}

export interface BudgetGridRow {
    budget: string;
}

export const emptyFeasibility: Feasibility = {
    id: '0',
    criteria: '',
    yearsBeforeAny: 0,
    yearsBeforeSame: 0,
};

export const emptyCost: Cost = {
    id: '0',
    equation: '',
    isFunction: false,
    criteria: ''
};

export const emptyConsequence: Consequence = {
    id: '0',
    attribute: '',
    change: '',
    equation: '',
    isFunction: false,
    criteria: ''
};

export const emptyTreatment: Treatment = {
    id: '0',
    name: '',
    feasibility: emptyFeasibility,
    costs: [],
    consequences: [],
    budgets: []
};

export const emptyTreatmentLibrary: TreatmentLibrary = {
    id: '0',
    name: '',
    description: '',
    treatments: []
};
