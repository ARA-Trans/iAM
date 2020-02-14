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
    weightingAttribute: string;
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
    benefitAttribute: '',
    weightingAttribute: ''
};

export interface ScenarioUser {
    id?: number;
    username?: string;
    canModify: boolean;
}

export interface Scenario {
    networkId: number;
    networkName: string;
    simulationId: number;
    simulationName: string;
    createdDate?: Date;
    lastModifiedDate?: Date;
    status?: string;
    shared?: boolean;
    owner?: string;
    creator?: string;
    id: number | string;
    users: ScenarioUser[];
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
    id: 0,
    users: []
};
