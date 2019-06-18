import {clone} from 'ramda';
import {emptyTreatment, emptyTreatmentLibrary, Treatment, TreatmentLibrary} from '@/shared/models/iAM/treatment';
import {emptyInvestmentLibrary, InvestmentLibrary} from '@/shared/models/iAM/investment';

export interface TabData {
    tabTreatmentLibraries: TreatmentLibrary[];
    tabSelectedTreatmentLibrary: TreatmentLibrary;
    tabSelectedTreatment: Treatment;
    tabScenarioInvestmentLibrary: InvestmentLibrary;
}

export const emptyTabData: TabData = {
    tabTreatmentLibraries: [],
    tabSelectedTreatmentLibrary: clone(emptyTreatmentLibrary),
    tabSelectedTreatment: clone(emptyTreatment),
    tabScenarioInvestmentLibrary: clone(emptyInvestmentLibrary)
};