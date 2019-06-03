export interface Analysis {
    id: number;
    startYear: number;
    analysisPeriod: number;
    optimizationType: string;
    budgetType: string;
    benefitLimit: number;
    description: string;
    criteria: string;
    benefitAttribute: string;
}

export const emptyAnalysis: Analysis = {
    id: 0,
    startYear: 0,
    analysisPeriod: 0,
    optimizationType: '',
    budgetType: '',
    benefitLimit: 0,
    description: '',
    criteria: '',
    benefitAttribute: ''
};

export interface Scenario {
    networkId: number;
    networkName: string;
    simulationId: number;
    simulationName: string;
    createdDate?: Date;
    lastModifiedDate?: Date;
    status?: string;
    shared?: boolean;
}

export const emptyScenario: Scenario = {
    networkId: 0,
    networkName: '',
    simulationId: 0,
    simulationName: '',
    createdDate: new Date(),
    lastModifiedDate: new Date(),
    status: '',
    shared: false,
};
