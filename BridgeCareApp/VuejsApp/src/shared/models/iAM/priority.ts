export interface PriorityFund {
    id: string;
    budget: string;
    funding: number;
}

export interface Priority {
    id: string;
    priorityLevel: number;
    year: number | null;
    criteria: string;
    priorityFunds: PriorityFund[];
}

export interface PriorityLibrary {
    id: string;
    name: string;
    owner?: string;
    shared?: boolean;
    description: string;
    priorities: Priority[];
}

export const emptyPriorityFund: PriorityFund = {
    id: '0',
    budget: '',
    funding: 0
};

export const emptyPriority: Priority = {
    id: '0',
    priorityLevel: 1,
    year: null,
    criteria: '',
    priorityFunds: []
};

export const emptyPriorityLibrary: PriorityLibrary = {
    id: '0',
    name: '',
    description: '',
    priorities: []
};

export interface PrioritiesDataTableRow {
    id: string;
    priorityLevel: string;
    year: string;
    criteria: string;

    [budgetName: string]: string;
}
