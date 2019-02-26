import * as R from 'ramda';
import {Criteria, CriteriaRule, CriteriaType} from '@/models/criteria';
import {hasValue} from '@/shared/utils/has-value';

/**
 * Creates a clause string from a given criteria object
 * @param criteria The criteria object used to create the clause string
 */
export const parseQueryBuilderJson = (criteria: Criteria) => {
    // create an empty string list to build the where clause
    let clause: string[] = [];
    if (!R.isNil(criteria) && !R.isEmpty(criteria.children)) {
        // set the logical operator
        const logicalOperator = ` ${criteria.logicalOperator} `;
        // @ts-ignore
        // loop over the criteria children and append all rules
        criteria.children.forEach((child: CriteriaType) => {
            if (child.type === 'query-builder-rule') {
                // create a clause rule from the query builder child.query
                const rule = parseQueryBuilderRule(child.query as CriteriaRule);
                if (hasValue(rule)) {
                    // append the logical operator if the string list is not empty
                    if (hasValue(clause)) {
                        clause.push(logicalOperator);
                    }
                    // append rule to string list
                    clause.push(rule);
                }
            } else {
                const clauseGroup = parseQueryBuilderJson(child.query as Criteria);
                const clauseGroupVal = clauseGroup.join('');
                if (hasValue(clauseGroupVal)) {
                    // append the logical operator if the string list is not empty
                    if (hasValue(clause)) {
                        clause.push(logicalOperator);
                    }
                    // recursively call this function to create a rule group
                    clause.push('(');
                    clause.push(...clauseGroup);
                    clause.push(')');
                }
            }
        });
    }
    return clause;
};
/**
 * Creates a clause rule substring from a given criteria rule object
 * @param criteriaRule The criteria rule object used to create the clause rule substring
 */
function parseQueryBuilderRule(criteriaRule: CriteriaRule) {
    // return the concatenated rule string
    return hasValue(criteriaRule.value)
        ? `[${criteriaRule.selectedOperand}]${criteriaRule.selectedOperator}'${criteriaRule.value}'`
        : '';
}


/**
 * Parses a clause string into a criteria object
 * @param clause The clause string to parse
 */
export const parseCriteriaString = (clause: string) => {
    // create a new criteria object
    const newCriteria: Criteria = {
        logicalOperator: '',
        children: []
    };
    // if no open parentheses are present, assume clause string was created in legacy app
    if ((clause.match(/\(/g) || []).length === 0) {
        // ensure there are no close parentheses in the string before parsing
        clause = clause.split(')').join(' ');
        // parse the clause string and return
        return parseLegacyAppClause(clause, newCriteria);
    } else {
        // parse the clause as a query builder string and return
        return parseQueryBuilderClause(clause, newCriteria);
    }
};

/**
 * Parses legacy app formatted clause string data into a criteria object
 * @param clause The clause string to parse
 * @param criteria The criteria object to parse the data into
 */
function parseLegacyAppClause(clause: string, criteria: Criteria) {
    const splitVals = clause.split(' ');
    let i = 0;
    while (i < splitVals.length) {
        const splitVal = splitVals[i];
        if (splitVal === 'AND' || splitVal === 'OR') {
            if (!hasValue(criteria.logicalOperator)) {
                // set criteria logical operator if it doesn't currently have one
                criteria.logicalOperator = splitVal;
            } else if (criteria.logicalOperator !== splitVal) {
                // create a new clause string starting at the current iteration to the end of the splitVals array
                const groupClause = splitVals.slice(i).join(' ');
                // create a new criteria object
                const newCriteria: Criteria = {
                    logicalOperator: '',
                    children: []
                };
                // create a new criteria type object, and recursively call the parseLegacyAppClause function with the
                // new clause and new criteria to create a query builder group of rules
                const criteriaType: CriteriaType = {
                    type: 'query-builder-group',
                    query: parseLegacyAppClause(groupClause, newCriteria)
                };
                // @ts-ignore
                // add the new criteria type to the current criteria
                criteria.children.push(criteriaType);
                // set current iteration value to the length of the current splitVals array and continue
                i = splitVals.length;
                continue;
            }
        } else {
            // create a new criteria rule by parsing the current substring
            const criteriaRule: CriteriaRule = parseCriteriaRule(splitVal);
            // create a new criteria type and add the new criteria rule to it
            const criteriaType: CriteriaType = {
                type: 'query-builder-rule',
                query: criteriaRule
            };
            // @ts-ignore
            // add the new criteria type to the current criteria
            criteria.children.push(criteriaType);
        }
        i++;
    }
    return criteria;
}

/**
 * Parses query builder formatted clause string into a criteria object
 * @param clause The clause string to parse
 * @param criteria The criteria object to parse the data into
 */
function parseQueryBuilderClause(clause: string, criteria: Criteria) {
    const splitVals = clause.split(' ');
    let i = 0;
    while (i < splitVals.length) {
        const splitVal = splitVals[i];
        if (splitVal.charAt(0) === '(') {
            // set groupEnd as the number of ( chars - the number of ) chars, 0 means the clause subgroup has been found
            let groupEnd = (splitVal.match(/\(/g) || []).length - (splitVal.match(/\)/g) || []).length;
            // set placeholder for clause substring
            let groupClause = '';
            if (groupEnd === 0) {
                // group contains only 1 operand/operator and no logical operators, so slice of the '(' & ')' chars at
                // start and end of the current iteration value
                groupClause = splitVal.slice(1, splitVal.length - 1);
                i++;
            } else {
                // start j at the next index of the substring list
                let j = i + 1;
                // loop over the substring list until groupEnd = 0
                while (groupEnd !== 0) {
                    // get current substring at substring list index j
                    const clauseSubstring = splitVals[j];
                    // add current groupEnd value to current substring number of '(' chars - the substring number of ')' chars
                    groupEnd = groupEnd + (clauseSubstring.match(/\(/g) || []).length
                        - (clauseSubstring.match(/\)/g) || []).length;
                    // if groupEnd != 0 then add 1 to j to continue to next substring in list
                    if (groupEnd !== 0) {
                        j++;
                    }
                }
                // slice the array at the current iteration to j + 1 index, then join with whitespace
                const joinedVals = splitVals.slice(i, j + 1).join(' ');
                // create a new clause string and slice off the '(' & ')' chars at the start and end
                groupClause = joinedVals.slice(1, joinedVals.length - 1);
                // set current iteration to j + 1 to begin iteration after the query group
                i = j + 1;
            }
            // create a new criteria object
            const newCriteria: Criteria = {
                logicalOperator: 'AND',
                children: []
            };
            // create a new criteria type object, and recursively call the parseCriteriaString function with the new clause
            // and new criteria to create a query builder group of rules
            const criteriaType: CriteriaType = {
                type: 'query-builder-group',
                query: parseQueryBuilderClause(groupClause, newCriteria)
            };
            // @ts-ignore
            // add the new criteria type to the current criteria
            criteria.children.push(criteriaType);
            // continue to next iteration
            continue;
        } else if (splitVal === 'AND' || splitVal === 'OR') {
            if (!hasValue(criteria.logicalOperator)) {
                // set logical operator for current criteria
                criteria.logicalOperator = splitVal;
            }
        } else {
            // create a new criteria rule by parsing the current substring
            const criteriaRule: CriteriaRule = parseCriteriaRule(splitVal);
            // create a new criteria type and add the new criteria rule to it
            const criteriaType: CriteriaType = {
                type: 'query-builder-rule',
                query: criteriaRule
            };
            // @ts-ignore
            // add the new criteria type to the current criteria
            criteria.children.push(criteriaType);
        }
        i++;
    }
    return criteria;
}
/**
 * Parses a clause substring into a criteria rule object
 * @param criteriaRuleString The clause substring to parse
 */
function parseCriteriaRule(criteriaRuleString: string): CriteriaRule {
    // get the operand value using [ & ] to find it in the rule string
    const operandStart = criteriaRuleString.indexOf('[') + 1;
    const operandEnd = criteriaRuleString.indexOf(']');
    const operand = criteriaRuleString.slice(operandStart, operandEnd);
    // create a string list for the operator
    const operator: string[] = [];
    // start checking the char after the ] in the rule string to get the operator strings, stop when you hit the start
    // of the value
    let charIndex = operandEnd + 1;
    while (criteriaRuleString.charAt(charIndex) !== '\'') {
        operator.push(criteriaRuleString.charAt(charIndex));
        charIndex++;
    }
    // get the value at the end of the rule string after the last index of the operator
    const value = criteriaRuleString.slice(charIndex + 1, criteriaRuleString.length - 1);
    // return object in shape of CriteriaRule
    return {
        rule: operand,
        selectedOperator: operator.join(''),
        selectedOperand: operand,
        value: value
    };
}
