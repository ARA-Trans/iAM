export interface IScenario {
    scenarioId: number;
    name: string;
    createdDate: Date;
    lastModifiedDate: Date;
    status: boolean;
    shared: boolean;
}

/***********************************************MOCK DATA**************************************************************/
export const userScenarios: IScenario[] = [
    {
        scenarioId: 1,
        name: 'Scenario A-1',
        createdDate: new Date(),
        lastModifiedDate: new Date(),
        status: true,
        shared: false
    },
    {
        scenarioId: 2,
        name: 'Scenario A-2',
        createdDate: new Date(),
        lastModifiedDate: new Date(),
        status: true,
        shared: false
    },
    {
        scenarioId: 3,
        name: 'Scenario A-3',
        createdDate: new Date(),
        lastModifiedDate: new Date(),
        status: false,
        shared: false
    },
    {
        scenarioId: 4,
        name: 'Scenario B-1',
        createdDate: new Date(),
        lastModifiedDate: new Date(),
        status: true,
        shared: false
    },
    {
        scenarioId: 5,
        name: 'Scenario B-2',
        createdDate: new Date(),
        lastModifiedDate: new Date(),
        status: true,
        shared: false
    },
    {
        scenarioId: 6,
        name: 'Scenario B-3',
        createdDate: new Date(),
        lastModifiedDate: new Date(),
        status: true,
        shared: false
    }
];

export const sharedScenarios: IScenario[] = [
    {
        scenarioId: 7,
        name: 'Scenario C-1',
        createdDate: new Date(),
        lastModifiedDate: new Date(),
        status: true,
        shared: true
    },
    {
        scenarioId: 8,
        name: 'Scenario C-2',
        createdDate: new Date(),
        lastModifiedDate: new Date(),
        status: true,
        shared: true
    },
    {
        scenarioId: 9,
        name: 'Scenario C-3',
        createdDate: new Date(),
        lastModifiedDate: new Date(),
        status: false,
        shared: true
    }
];
