export interface PriorityFund {
    priorityId: string;
    id: string;
    budget: string;
    funding: number;
}

export interface Priority {
    scenarioId: number;
    id: string;
    priorityLevel: number;
    year: number;
    criteria: string;
    priorityFunds: PriorityFund[];
}

export const emptyPriorityFund: PriorityFund = {
    priorityId: '0',
    id: '0',
    budget: '',
    funding: 0
};

export const emptyPriority: Priority = {
    scenarioId: 0,
    id: '0',
    priorityLevel: 1,
    year: 0,
    criteria: '',
    priorityFunds: []
};

export interface PrioritiesDataTableRow {
    priorityId: string;
    priorityLevel: string;
    year: string;
    criteria: string;
    [budgetName: string]: string;
}
