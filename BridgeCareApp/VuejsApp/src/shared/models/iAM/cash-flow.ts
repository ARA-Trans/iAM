export interface CashFlowParameter {
    id: string;
    parameter: string;
    criteria: string;
}

export interface CashFlowDuration {
    id: string;
    duration: number;
    maxTreatmentCost: string;
}

export interface CashFlowLibrary {
    id: string;
    name: string;
    description: string;
    parameters: CashFlowParameter[];
    durations: CashFlowDuration[];
}

export interface Parameter {
    id: string;
    name: string;
}

export const emptyCashFlowLibrary: CashFlowLibrary = {
    id: '0',
    name: '',
    description: '',
    parameters: [],
    durations: []
};

export const emptyCashFlowParameter: CashFlowParameter = {
    id: '0',
    parameter: '',
    criteria: ''
};

export const emptyCashFlowDuration: CashFlowDuration = {
    id: '0',
    duration: 1,
    maxTreatmentCost: ''
};

const ObjectID = require('bson-objectid');

export const parameterDummyData: Parameter[] = [
    {id: ObjectID.generate(), name: 'District 1'},
    {id: ObjectID.generate(), name: 'District 2'},
    {id: ObjectID.generate(), name: 'District 3'},
    {id: ObjectID.generate(), name: 'District 4'},
    {id: ObjectID.generate(), name: 'District 5'}
];