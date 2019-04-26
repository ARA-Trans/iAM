export interface Analysis {
    scenarioId: number;
    startYear: number;
    analysisPeriod: number;
    optimizationType: string;
    budgetType: string;
    benefitLimit: number;
    description: string;
    criteria: string;
}

export const emptyAnalysis: Analysis = {
    scenarioId: 0,
    startYear: 0,
    analysisPeriod: 0,
    optimizationType: '',
    budgetType: '',
    benefitLimit: 0,
    description: '',
    criteria: ''
};

export interface Scenario {
    networkId: number;
    simulationId: number;
    networkName: string;
    simulationName: string;
    name: string;
    createdDate: Date;
    lastModifiedDate: Date;
    status: string;
    shared: boolean;
}
