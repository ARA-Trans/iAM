export interface PriorityFund {
    priorityId: number;
    id: number;
    budget: string;
    funding: number;
}

export interface Priority {
    scenarioId: number;
    id: number;
    priorityLevel: number;
    year: number;
    criteria: string;
    priorityFunds: PriorityFund[];
}

export const emptyPriorityFund: PriorityFund = {
    priorityId: 0,
    id: 0,
    budget: '',
    funding: 0
};

export const emptyPriority: Priority = {
    scenarioId: 0,
    id: 0,
    priorityLevel: 0,
    year: 0,
    criteria: '',
    priorityFunds: []
};

export interface PrioritiesDataTableRow {
    priorityId: number;
    priorityLevel: number;
    year: number;
    // @ts-ignore
    criteria: string;
    [budgetName: string]: number;
}
