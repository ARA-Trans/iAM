import * as R from 'ramda';
import {Criteria, CriteriaRule, CriteriaType} from '@/models/criteria';
import {hasValue} from '@/shared/utils/has-value';

export const parseQueryBuilderJson = (criteria: Criteria) => {
    // create an empty string list to build the where clause
    let clause: string[] = [];
    if (!R.isNil(criteria) && !R.isEmpty(criteria.children)) {
        // set the logical operator
        const logicalOperator = ` ${criteria.logicalOperator} `;
        // @ts-ignore
        // loop over the criteria children and append all rules
        criteria.children.forEach((child: CriteriaType) => {
            // append the logical operator if the string list is not empty
            if (hasValue(clause)) {
                clause.push(logicalOperator);
            }
            // append rule to string list
            if (child.type === 'query-builder-rule') {
                const rule = createRule(child.query as CriteriaRule);
                clause.push(rule);
            } else {
                // recursively call this function to create a rule group
                clause.push('(');
                clause.push(...parseQueryBuilderJson(child.query as Criteria));
                clause.push(')');
            }
        });
    }
    // return the string list
    return clause;
};

// Appends criteria rules to a string list
function createRule(criteriaRule: CriteriaRule) {
    // get the criteria rule value
    // @ts-ignore
    const value = isNaN(parseInt(criteriaRule.value, 10))
        ? `'${criteriaRule.value}'`
        : criteriaRule.value;
    // return the concatenated rule string
    return `[${criteriaRule.selectedOperand}]${criteriaRule.selectedOperator}${value}`;
}
