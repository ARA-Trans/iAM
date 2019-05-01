export interface CreateScenario {
    name: string;
    description: string;
}

export const emptyScenario: CreateScenario = {
    name: '',
    description: ''
};

export interface CreateScenarioDialogData {
    showDialog: boolean;
    description: string;
}