import {equals} from 'ramda';
import {emptyInvestmentLibrary} from '@/shared/models/iAM/investment';
import {emptyPerformanceLibrary} from '@/shared/models/iAM/performance';
import {emptyTreatmentLibrary, Treatment} from '@/shared/models/iAM/treatment';
import {emptyPriorityLibrary} from '@/shared/models/iAM/priority';
import {emptyTargetLibrary} from '@/shared/models/iAM/target';
import {emptyDeficientLibrary} from '@/shared/models/iAM/deficient';
import {emptyRemainingLifeLimitLibrary} from '@/shared/models/iAM/remaining-life-limit';
import {emptyCashFlowLibrary} from '@/shared/models/iAM/cash-flow';
import {sorter} from '@/shared/utils/sorter-utils';

export const hasUnsavedChanges = (editor: string, localSelectedLibrary: any, stateSelectedLibrary: any, stateScenarioLibrary: any) => {
    switch (editor) {
        case 'investment':
            return !equals(localSelectedLibrary, emptyInvestmentLibrary) &&
                !equals(localSelectedLibrary, stateSelectedLibrary) &&
                !equals(localSelectedLibrary, stateScenarioLibrary);
        case 'performance':
            return !equals(localSelectedLibrary, emptyPerformanceLibrary) &&
                !equals(localSelectedLibrary, stateSelectedLibrary) &&
                !equals(localSelectedLibrary, stateScenarioLibrary);
        case 'treatment':
            localSelectedLibrary = {
                ...localSelectedLibrary,
                treatments: localSelectedLibrary.treatments.map((treatment: Treatment) => ({
                    ...treatment,
                    budgets: sorter(treatment.budgets)
                }))
            };
            stateSelectedLibrary = {
                ...stateSelectedLibrary,
                treatments: stateSelectedLibrary.treatments.map((treatment: Treatment) => ({
                    ...treatment,
                    budgets: sorter(treatment.budgets)
                }))
            };
            stateScenarioLibrary = {
                ...stateScenarioLibrary,
                treatments: stateScenarioLibrary.treatments.map((treatment: Treatment) => ({
                    ...treatment,
                    budgets: sorter(treatment.budgets)
                }))
            };
            return !equals(localSelectedLibrary, emptyTreatmentLibrary) &&
                !equals(localSelectedLibrary, stateSelectedLibrary) &&
                !equals(localSelectedLibrary, stateScenarioLibrary);
        case 'priority':
            return !equals(localSelectedLibrary, emptyPriorityLibrary) &&
                !equals(localSelectedLibrary, stateSelectedLibrary) &&
                !equals(localSelectedLibrary, stateScenarioLibrary);
        case 'target':
            return !equals(localSelectedLibrary, emptyTargetLibrary) &&
                !equals(localSelectedLibrary, stateSelectedLibrary) &&
                !equals(localSelectedLibrary, stateScenarioLibrary);
        case 'deficient':
            return !equals(localSelectedLibrary, emptyDeficientLibrary) &&
                !equals(localSelectedLibrary, stateSelectedLibrary) &&
                !equals(localSelectedLibrary, stateScenarioLibrary);
        case 'remaininglifelimit':
            return !equals(localSelectedLibrary, emptyRemainingLifeLimitLibrary) &&
                !equals(localSelectedLibrary, stateSelectedLibrary) &&
                !equals(localSelectedLibrary, stateScenarioLibrary);
        case 'cashflow':
            return !equals(localSelectedLibrary, emptyCashFlowLibrary) &&
                !equals(localSelectedLibrary, stateSelectedLibrary) &&
                !equals(localSelectedLibrary, stateScenarioLibrary);
        default:
            return false;
    }
};
