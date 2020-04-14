import {emptyInvestmentLibrary} from '@/shared/models/iAM/investment';
import {emptyPerformanceLibrary} from '@/shared/models/iAM/performance';
import {emptyTreatmentLibrary} from '@/shared/models/iAM/treatment';
import {emptyPriorityLibrary} from '@/shared/models/iAM/priority';
import {emptyTargetLibrary} from '@/shared/models/iAM/target';
import {emptyDeficientLibrary} from '@/shared/models/iAM/deficient';
import {emptyRemainingLifeLimitLibrary} from '@/shared/models/iAM/remaining-life-limit';
import {emptyCashFlowLibrary} from '@/shared/models/iAM/cash-flow';
import {emptyCriteriaLibrary} from '@/shared/models/iAM/criteria';
import {clone, isEmpty, keys, symmetricDifference} from 'ramda';
import {hasValue} from '@/shared/utils/has-value-util';
import {sorter} from '@/shared/utils/sorter-utils';

export const hasUnsavedChanges = (editor: string, localSelectedLibrary: any, stateSelectedLibrary: any, stateScenarioLibrary: any) => {
    const localLibrary = sortNonObjectLists(clone(localSelectedLibrary));
    const selectedLibrary = sortNonObjectLists(clone(stateSelectedLibrary));
    const scenarioLibrary = sortNonObjectLists(clone(stateScenarioLibrary));

    switch (editor) {
        case 'investment':
            return !isEqual(localLibrary, emptyInvestmentLibrary) &&
                !isEqual(localLibrary, selectedLibrary) &&
                !isEqual(localLibrary, scenarioLibrary);
        case 'performance':
            return !isEqual(localLibrary, emptyPerformanceLibrary) &&
                !isEqual(localLibrary, selectedLibrary) &&
                !isEqual(localLibrary, scenarioLibrary);
        case 'treatment':
            return !isEqual(localLibrary, emptyTreatmentLibrary) &&
                !isEqual(localLibrary, selectedLibrary) &&
                !isEqual(localLibrary, scenarioLibrary);
        case 'priority':
            return !isEqual(localLibrary, emptyPriorityLibrary) &&
                !isEqual(localLibrary, selectedLibrary) &&
                !isEqual(localLibrary, scenarioLibrary);
        case 'target':
            return !isEqual(localLibrary, emptyTargetLibrary) &&
                !isEqual(localLibrary, selectedLibrary) &&
                !isEqual(localLibrary, scenarioLibrary);
        case 'deficient':
            return !isEqual(localLibrary, emptyDeficientLibrary) &&
                !isEqual(localLibrary, selectedLibrary) &&
                !isEqual(localLibrary, scenarioLibrary);
        case 'remaininglifelimit':
            return !isEqual(localLibrary, emptyRemainingLifeLimitLibrary) &&
                !isEqual(localLibrary, selectedLibrary) &&
                !isEqual(localLibrary, scenarioLibrary);
        case 'cashflow':
            return !isEqual(localLibrary, emptyCashFlowLibrary) &&
                !isEqual(localLibrary, selectedLibrary) &&
                !isEqual(localLibrary, scenarioLibrary);
        case 'criteria':
            return !isEqual(localLibrary, emptyCriteriaLibrary) &&
                !isEqual(localLibrary, selectedLibrary);
        default:
            return false;
    }
};

export const sortNonObjectLists = (item: any) => {
    if (hasValue(item)) {
        keys(item).forEach((prop) => {
            if (Array.isArray(item[prop])) {
                if (item[prop].every((arrItem: any) => typeof arrItem === 'object')) {
                    item[prop] = item[prop].map((arrItem: any) => sortNonObjectLists(arrItem));
                } else {
                    item[prop] = sorter(item[prop]);
                }
            }
        });
    }

    return item;
};

export const isEqual = (item1: any, item2: any) => {
    return isEmpty(symmetricDifference([item1], [item2]));
};
