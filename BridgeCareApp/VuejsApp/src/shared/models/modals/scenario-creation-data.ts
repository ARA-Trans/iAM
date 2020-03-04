export interface ScenarioCreationData {
    networkId: number;
    name: string;
    owner?: string;
    creator: string;
}

export const emptyCreateScenarioData: ScenarioCreationData = {
    networkId: 0,
    name: '',
    creator: ''
};
