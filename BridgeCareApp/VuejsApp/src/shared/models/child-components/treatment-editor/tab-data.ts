import {clone} from 'ramda';
import {emptyTreatment, emptyTreatmentLibrary, Treatment, TreatmentLibrary} from '@/shared/models/iAM/treatment';
import {emptyInvestmentLibrary, InvestmentLibrary} from '@/shared/models/iAM/investment';

export interface TabData {
    tabTreatmentLibraries: TreatmentLibrary[];
    tabSelectedTreatmentLibrary: TreatmentLibrary;
    tabSelectedTreatment: Treatment;
    tabScenarioInvestmentLibrary: InvestmentLibrary;
    latestFeasibilityId: number;
    latestCostId: number;
    latestConsequenceId: number;
}

export const emptyTabData: TabData = {
    tabTreatmentLibraries: [],
    tabSelectedTreatmentLibrary: clone(emptyTreatmentLibrary),
    tabSelectedTreatment: clone(emptyTreatment),
    tabScenarioInvestmentLibrary: clone(emptyInvestmentLibrary),
    latestFeasibilityId: 0,
    latestCostId: 0,
    latestConsequenceId: 0
};