export interface SplitTreatmentLimit {
    id: string;
    rank: number;
    amount: number | null;
    percentage: string;
}

export interface SplitTreatment {
    id: string;
    description: string;
    criteria: string;
    splitTreatmentLimits: SplitTreatmentLimit[];
}

export interface CashFlowLibrary {
    id: string;
    name: string;
    owner?: string;
    shared?: boolean;
    description: string;
    splitTreatments: SplitTreatment[];
}

export const emptyCashFlowLibrary: CashFlowLibrary = {
    id: '0',
    name: '',
    description: '',
    splitTreatments: []
};

export const emptySplitTreatment: SplitTreatment = {
    id: '0',
    description: '',
    criteria: '',
    splitTreatmentLimits: []
};

export const emptySplitTreatmentLimit: SplitTreatmentLimit = {
    id: '0',
    rank: 1,
    amount: 1000000,
    percentage: '100'
};
