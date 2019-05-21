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
    scenarioId: number;
    networkName: string;
    simulationName: string;
    name: string;
    createdDate: Date;
    lastModifiedDate: Date;
    status: string;
    shared: boolean;
}

export const emptyScenario: Scenario = {
    networkId: 0,
    scenarioId: 0,
    networkName: '',
    simulationName: '',
    name: '',
    createdDate: new Date(),
    lastModifiedDate: new Date(),
    status: '',
    shared: false
};
