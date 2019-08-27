export interface RemainingLifeLimit {
    id: number | string;
    attribute: string;
    limit: number;
    criteria: string;
}

export const emptyRemainingLifeLimit: RemainingLifeLimit = {
    id: 0,
    attribute: '',
    limit: 0,
    criteria: ''
};

export interface RemainingLifeLimitLibrary {
    id: number | string;
    name: string;
    description: string;
    remainingLifeLimits: RemainingLifeLimit[];
}

export const emptyRemainingLifeLimitLibrary: RemainingLifeLimitLibrary = {
    id: 0,
    name: '',
    description: '',
    remainingLifeLimits: []
};
