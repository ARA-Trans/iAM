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