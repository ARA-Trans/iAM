export interface Feasibility {
    treatmentId: number;
    id: number;
    criteria: string;
    yearsBeforeAny: number;
    yearsBeforeSame: number;
}

export interface Cost {
    treatmentId: number;
    id: number;
    equation: string;
    isFunction: boolean;
    criteria: string;
}

export interface Consequence {
    treatmentId: number;
    id: number;
    attribute: string;
    change: string;
    equation: string;
    isFunction: boolean;
    criteria: string;
}

export interface Treatment {
    treatmentLibraryId: number;
    id: number;
    name: string;
    feasibility: Feasibility | null;
    costs: Cost[];
    consequences: Consequence[];
    budgets: string[];
}

export interface TreatmentLibrary {
    id: number;
    name: string;
    description: string;
    treatments: Treatment[];
}

export const emptyTreatmentLibrary: TreatmentLibrary = {
    id: 0,
    name: '',
    description: '',
    treatments: []
};

export const emptyTreatment: Treatment = {
    treatmentLibraryId: 0,
    id: 0,
    name: '',
    feasibility: null,
    costs: [],
    consequences: [],
    budgets: []
};

export const emptyFeasibility: Feasibility = {
    treatmentId: 0,
    id: 0,
    criteria: '',
    yearsBeforeAny: 0,
    yearsBeforeSame: 0,
};

export const emptyCost: Cost = {
    treatmentId: 0,
    id: 0,
    equation: '',
    isFunction: false,
    criteria: ''
};

export const emptyConsequence: Consequence = {
    treatmentId: 0,
    id: 0,
    attribute: '',
    change: '',
    equation: '',
    isFunction: false,
    criteria: ''
};
