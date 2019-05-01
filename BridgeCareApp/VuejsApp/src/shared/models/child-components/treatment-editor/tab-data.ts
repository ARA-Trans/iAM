import {emptyTreatment, emptyTreatmentLibrary, Treatment, TreatmentLibrary} from '@/shared/models/iAM/treatment';

export interface TabData {
    tabTreatmentLibraries: TreatmentLibrary[];
    tabSelectedTreatmentLibrary: TreatmentLibrary;
    tabSelectedTreatment: Treatment;
    latestFeasibilityId: number;
    latestCostId: number;
    latestConsequenceId: number;
}

export const emptyTabData: TabData = {
    tabTreatmentLibraries: [],
    tabSelectedTreatmentLibrary: {...emptyTreatmentLibrary},
    tabSelectedTreatment: {...emptyTreatment},
    latestFeasibilityId: 0,
    latestCostId: 0,
    latestConsequenceId: 0
};