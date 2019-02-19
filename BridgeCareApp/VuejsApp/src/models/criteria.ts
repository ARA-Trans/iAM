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

export const emptyCriteria: Criteria = {
    logicalOperator: 'all',
    children: []
};
