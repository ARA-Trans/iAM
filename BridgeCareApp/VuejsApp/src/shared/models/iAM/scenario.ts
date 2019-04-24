export interface Analysis {
    startYear: number;
    analysisPeriod: number;
    optimizationType: string;
    budgetType: string;
    benefitLimit: number;
    description: string;
    criteria: string;
}

export const emptyAnalysis: Analysis = {
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
    analysis: Analysis;
}

export const emptyScenario: Scenario = {
    networkId: 0,
    simulationId: 0,
    networkName: '',
    simulationName: '',
    name: '',
    createdDate: new Date(),
    lastModifiedDate: new Date(),
    status: '',
    shared: false,
    analysis: {...emptyAnalysis} as Analysis
};

export interface ScenarioAnalysisUpsertData {
    id: number;
    analysis: Analysis;
}
