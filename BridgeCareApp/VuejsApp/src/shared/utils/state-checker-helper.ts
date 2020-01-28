import {equals, find, propEq} from 'ramda';
import {emptyInvestmentLibrary, InvestmentLibrary} from '@/shared/models/iAM/investment';
import {hasValue} from '@/shared/utils/has-value-util';
import {emptyPerformanceLibrary, PerformanceLibrary} from '@/shared/models/iAM/performance';
import {emptyTreatmentLibrary, TreatmentLibrary} from '@/shared/models/iAM/treatment';
import {emptyPriorityLibrary, PriorityLibrary} from '@/shared/models/iAM/priority';
import {emptyTargetLibrary, TargetLibrary} from '@/shared/models/iAM/target';
import {DeficientLibrary, emptyDeficientLibrary} from '@/shared/models/iAM/deficient';
import {emptyRemainingLifeLimitLibrary, RemainingLifeLimitLibrary} from '@/shared/models/iAM/remaining-life-limit';
import {CashFlowLibrary, emptyCashFlowLibrary} from '@/shared/models/iAM/cash-flow';

export const checkStateForUnsavedChanges = (fromPath: string, state: any) => {
    switch (fromPath) {
        case '/EditAnalysis/':
            return false;
        case '/InvestmentEditor/Scenario/':
        case '/InvestmentEditor/Library/':
            return investmentEditorStateChecker(fromPath, state);
        case '/PerformanceEditor/Scenario/':
        case '/PerformanceEditor/Library/':
            return performanceEditorStateChecker(fromPath, state);
        case '/TreatmentEditor/Scenario/':
        case '/TreatmentEditor/Library/':
            return treatmentEditorStateChecker(fromPath, state);
        case '/PriorityEditor/Scenario/':
        case '/PriorityEditor/Library/':
            return priorityEditorStateChecker(fromPath, state);
        case '/TargetEditor/Scenario/':
        case '/TargetEditor/Library/':
            return targetEditorStateChecker(fromPath, state);
        case '/DeficientEditor/Scenario/':
        case '/DeficientEditor/Library/':
            return deficientEditorStateChecker(fromPath, state);
        case '/RemainingLifeLimitEditor/Scenario/':
        case '/RemainingLifeLimitEditor/Library/':
            return remainingLifeLimitEditorStateChecker(fromPath, state);
        case '/CashFlowEditor/Scenario':
        case '/CashFlowEditor/Library/':
            return cashFlowEditorStateChecker(fromPath, state);
        default:
            return false;
    }
};

const investmentEditorStateChecker = (fromPath: string, state: any) => {
    const selectedLibrary: InvestmentLibrary = state.investmentEditor.selectedInvestmentLibrary;

    switch (fromPath) {
        case '/InvestmentEditor/Scenario/':
            const scenarioLibrary: InvestmentLibrary = state.investmentEditor.scenarioInvestmentLibrary;
            return !equals(selectedLibrary, emptyInvestmentLibrary) && !equals(selectedLibrary, scenarioLibrary);
        case '/InvestmentEditor/Library/':
            const library: InvestmentLibrary = find(propEq('id', selectedLibrary.id), state.investmentEditor.investmentLibraries);
            return hasValue(library) && !equals(selectedLibrary, library);
        default:
            return false;
    }
};

const performanceEditorStateChecker = (fromPath: string, state: any) => {
    const selectedLibrary: PerformanceLibrary = state.performanceEditor.selectedPerformanceLibrary;

    switch (fromPath) {
        case '/PerformanceEditor/Scenario/':
            const scenarioLibrary: PerformanceLibrary = state.performanceEditor.scenarioPerformanceLibrary;
            return !equals(selectedLibrary, emptyPerformanceLibrary) && !equals(selectedLibrary, scenarioLibrary);
        case '/PerformanceEditor/Library/':
            const library: PerformanceLibrary = find(propEq('id', selectedLibrary.id), state.performanceEditor.performanceLibraries);
            return hasValue(library) && !equals(selectedLibrary, library);
        default:
            return false;
    }
};

const treatmentEditorStateChecker = (fromPath: string, state: any) => {
    const selectedLibrary: TreatmentLibrary = state.treatmentEditor.selectedTreatmentLibrary;

    switch (fromPath) {
        case '/TreatmentEditor/Scenario/':
            const scenarioLibrary: TreatmentLibrary = state.treatmentEditor.scenarioTreatmentLibrary;
            return !equals(selectedLibrary, emptyTreatmentLibrary) && !equals(selectedLibrary, scenarioLibrary);
        case '/TreatmentEditor/Library/':
            const library: TreatmentLibrary = find(propEq('id', selectedLibrary.id), state.treatmentEditor.treatmentLibraries);
            return hasValue(library) && !equals(selectedLibrary, library);
        default:
            return false;
    }
};

const priorityEditorStateChecker = (fromPath: string, state: any) => {
    const selectedLibrary: PriorityLibrary = state.priorityEditor.selectedPriorityLibrary;

    switch (fromPath) {
        case '/PriorityEditor/Scenario/':
            const scenarioLibrary: PriorityLibrary = state.priorityEditor.scenarioPriorityLibrary;
            return !equals(selectedLibrary, emptyPriorityLibrary) && !equals(selectedLibrary, scenarioLibrary);
        case '/PriorityEditor/Library/':
            const library: PriorityLibrary = find(propEq('id', selectedLibrary.id), state.priorityEditor.priorityLibraries);
            return hasValue(library) && !equals(selectedLibrary, library);
        default:
            return false;
    }
};

const targetEditorStateChecker = (fromPath: string, state: any) => {
    const selectedLibrary: TargetLibrary = state.targetEditor.selectedTargetLibrary;

    switch (fromPath) {
        case '/TargetEditor/Scenario/':
            const scenarioLibrary: TargetLibrary = state.targetEditor.scenarioTargetLibrary;
            return !equals(selectedLibrary, emptyTargetLibrary) && !equals(selectedLibrary, scenarioLibrary);
        case '/TargetEditor/Library/':
            const library: TargetLibrary = find(propEq('id', selectedLibrary.id), state.targetEditor.targetLibraries);
            return hasValue(library) && !equals(selectedLibrary, library);
        default:
            return false;
    }
};

const deficientEditorStateChecker = (fromPath: string, state: any) => {
    const selectedLibrary: DeficientLibrary = state.deficientEditor.selectedDeficientLibrary;

    switch (fromPath) {
        case '/DeficientEditor/Scenario/':
            const scenarioLibrary: DeficientLibrary = state.deficientEditor.scenarioDeficientLibrary;
            return !equals(selectedLibrary, emptyDeficientLibrary) && !equals(selectedLibrary, scenarioLibrary);
        case '/DeficientEditor/Library/':
            const library: DeficientLibrary = find(propEq('id', selectedLibrary.id), state.deficientEditor.deficientLibraries);
            return hasValue(library) && !equals(selectedLibrary, library);
        default:
            return false;
    }
};

const remainingLifeLimitEditorStateChecker = (fromPath: string, state: any) => {
    const selectedLibrary: RemainingLifeLimitLibrary = state.remainingLifeLimitEditor.selectedRemainingLifeLimitLibrary;

    switch (fromPath) {
        case '/RemainingLifeLimitEditor/Scenario/':
            const scenarioLibrary: RemainingLifeLimitLibrary = state.remainingLifeLimitEditor.scenarioRemainingLifeLimitLibrary;
            return !equals(selectedLibrary, emptyRemainingLifeLimitLibrary) && !equals(selectedLibrary, scenarioLibrary);
        case '/RemainingLifeLimitEditor/Library/':
            const library: RemainingLifeLimitLibrary = find(propEq('id', selectedLibrary.id), state.remainingLifeLimitEditor.remainingLifeLimitLibraries);
            return hasValue(library) && !equals(selectedLibrary, library);
        default:
            return false;
    }
};

const cashFlowEditorStateChecker = (fromPath: string, state: any) => {
    const selectedLibrary: CashFlowLibrary = state.cashFlowEditor.selectedCashFlowLibrary;

    switch (fromPath) {
        case '/CashFlowEditor/Scenario/':
            const scenarioLibrary: CashFlowLibrary = state.cashFlowEditor.scenarioCashFlowLibrary;
            return !equals(selectedLibrary, emptyCashFlowLibrary) && !equals(selectedLibrary, scenarioLibrary);
        case '/CashFlowEditor/Library/':
            const library: CashFlowLibrary = find(propEq('id', selectedLibrary.id), state.cashFlowEditor.cashFlowLibraries);
            return hasValue(library) && !equals(selectedLibrary, library);
        default:
            return false;
    }
};