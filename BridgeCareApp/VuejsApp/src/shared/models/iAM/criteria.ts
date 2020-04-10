export interface CriteriaEditorAttribute {
    name: string;
    values: string[];
}

export interface Criteria {
    logicalOperator: string;
    children?: CriteriaType[];
}

export interface CriteriaType {
    type: string;
    query: Criteria | CriteriaRule;
}

export interface CriteriaRule {
    rule: string;
    selectedOperator?: string;
    selectedOperand: string;
    value?: string;
}

export interface CriteriaValidationResult {
    isValid: boolean;
    numberOfResults: number;
    message: string;
}

export interface CriteriaLibrary {
    id: string;
    name: string;
    description: string;
    criteria: string;
    owner?: string;
    shared?: boolean;
}

export interface CriteriaEditorData {
    mainCriteriaString: string;
    isLibraryContext: boolean;
}

export interface CriteriaEditorResult {
    validated: boolean;
    criteria: string | null;
}

export const emptyCriteria: Criteria = {
    logicalOperator: 'AND',
    children: []
};

export const emptyCriteriaLibrary: CriteriaLibrary = {
    id: '0',
    name: '',
    description: '',
    criteria: ''
};

export const emptyCriteriaEditorData: CriteriaEditorData = {
    mainCriteriaString: '',
    isLibraryContext: false
};
