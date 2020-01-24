import {equals, find, propEq} from 'ramda';
import {emptyInvestmentLibrary, InvestmentLibrary} from '@/shared/models/iAM/investment';
import {hasValue} from '@/shared/utils/has-value-util';

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
    switch (fromPath) {
        case '/PerformanceEditor/Scenario/':
            break;
        case '/PerformanceEditor/Library/':
            break;
        default:
            return false;
    }
};

const treatmentEditorStateChecker = (fromPath: string, state: any) => {
    switch (fromPath) {
        case '/TreatmentEditor/Scenario/':
            break;
        case '/TreatmentEditor/Library/':
            break;
        default:
            return false;
    }
};

const priorityEditorStateChecker = (fromPath: string, state: any) => {
    switch (fromPath) {
        case '/PriorityEditor/Scenario/':
            break;
        case '/PriorityEditor/Library/':
            break;
        default:
            return false;
    }
};

const targetEditorStateChecker = (fromPath: string, state: any) => {
    switch (fromPath) {
        case '/TargetEditor/Scenario/':
            break;
        case '/TargetEditor/Library/':
            break;
        default:
            return false;
    }
};

const deficientEditorStateChecker = (fromPath: string, state: any) => {
    switch (fromPath) {
        case '/DeficientEditor/Scenario/':
            break;
        case '/DeficientEditor/Library/':
            break;
        default:
            return false;
    }
};

const remainingLifeLimitEditorStateChecker = (fromPath: string, state: any) => {
    switch (fromPath) {
        case '/RemainingLifeLimitEditor/Scenario/':
            break;
        case '/RemainingLifeLimitEditor/Library/':
            break;
        default:
            return false;
    }
};

const cashFlowEditorStateChecker = (fromPath: string, state: any) => {
    switch (fromPath) {
        case '/CashFlowEditor/Scenario/':
            break;
        case '/CashFlowEditor/Library/':
            break;
        default:
            return false;
    }
};