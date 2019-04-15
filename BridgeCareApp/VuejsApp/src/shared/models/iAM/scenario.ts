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
