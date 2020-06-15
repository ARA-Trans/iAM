import {hasValue} from '@/shared/utils/has-value-util';
import {SplitTreatment, SplitTreatmentLimit} from '@/shared/models/iAM/cash-flow';
import {findIndex, propEq, contains} from 'ramda';
import {getPropertyValues} from '@/shared/utils/getter-utils';
import {CriteriaDrivenBudget} from '@/shared/models/iAM/investment';

export interface InputValidationRules {
    [Rules: string]: any;
}

/***********************************************GENERAL RULES**********************************************************/
const generalRules = {
    'valueIsNotEmpty': (value: any) => {
        return hasValue(value) || 'Value cannot be empty';
    },
    'valueIsWithinRange': (value: number, range: number[]) => {
        return (value >= range[0] && value <= range[1]) || `Value must be in range ${range[0]} - ${range[1]}`;
    }
};
/***********************************************CASH FLOW RULES********************************************************/
const cashFlowRules = {
    'isRankGreaterThanPreviousRank': (splitTreatmentLimit: SplitTreatmentLimit, selectedSplitTreatment: SplitTreatment) => {
        const index: number = findIndex(
            propEq('id', splitTreatmentLimit.id), selectedSplitTreatment.splitTreatmentLimits);

        if (index > 0) {
            return splitTreatmentLimit.rank > selectedSplitTreatment.splitTreatmentLimits[index - 1].rank ||
                'Value must be greater than previous value';
        }

        return true;
    },
    'isAmountGreaterThanOrEqualToPreviousAmount': (splitTreatmentLimit: SplitTreatmentLimit, selectedSplitTreatment: SplitTreatment) => {
        const index: number = findIndex(
            propEq('id', splitTreatmentLimit.id), selectedSplitTreatment.splitTreatmentLimits);

        if (index > 0) {
            const currentAmount: number | null = hasValue(splitTreatmentLimit.amount)
                ? parseFloat(splitTreatmentLimit.amount!.toString().replace(/(\$*)(\,*)/g, ''))
                : null;

            const previousAmount: number | null = hasValue(selectedSplitTreatment.splitTreatmentLimits[index - 1].amount)
                ? selectedSplitTreatment.splitTreatmentLimits[index - 1].amount!
                : null;

            return !hasValue(currentAmount) || (hasValue(currentAmount) && !hasValue(previousAmount)) ||
                (hasValue(currentAmount) && hasValue(previousAmount) && currentAmount! >= previousAmount!) ||
                'Value must be greater than or equal to previous value';
        }

        return true;
    },
    'doesTotalOfPercentsEqualOneHundred': (values: string) => {
        let total: number = 0;

        if (values.indexOf('/')) {
            const percents: number[] = values.split('/').map((value: string) => parseInt(value));
            total = percents.reduce((x: number, y: number) => x + y);
        }

        return total === 100 || 'Total of percents must equal 100';
    }
};
/**********************************************INVESTMENT RULES********************************************************/
const investmentRules = {
    'budgetNameIsUnique': (budget: CriteriaDrivenBudget, budgets: CriteriaDrivenBudget[]) => {
        const otherBudgetNames: string[] = getPropertyValues(
            'budgetName', budgets.filter((b: CriteriaDrivenBudget) => b.id !== budget.id));
        return !contains(budget.budgetName, otherBudgetNames) || 'Budget name must be unique';
    }
};
/***********************************************TREATMENT RULES********************************************************/
const treatmentRules = {
    'changeHasEquation': (change: string, equation: string) => {
        if (!hasValue(change)) {
            return hasValue(equation) || 'Must have an equation to be blank';
        }
        return true;
    }
};
/**************************************************ALL RULES***********************************************************/
export const rules: InputValidationRules = {
    'generalRules': generalRules,
    'cashFlowRules': cashFlowRules,
    'investmentRules': investmentRules,
    'treatmentRules': treatmentRules
};