import {emptyTreatment, emptyTreatmentLibrary, Treatment, TreatmentLibrary} from '@/shared/models/iAM/treatment';

export interface TabData {
    tabTreatmentLibraries: TreatmentLibrary[];
    tabSelectedTreatmentLibrary: TreatmentLibrary;
    tabSelectedTreatment: Treatment;
}

export const emptyTabData: TabData = {
    tabTreatmentLibraries: [],
    tabSelectedTreatmentLibrary: {...emptyTreatmentLibrary},
    tabSelectedTreatment: {...emptyTreatment}
};