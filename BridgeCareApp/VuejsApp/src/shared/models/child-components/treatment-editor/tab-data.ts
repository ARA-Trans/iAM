import {emptyTreatment, emptyTreatmentStrategy, Treatment, TreatmentStrategy} from '@/shared/models/iAM/treatment';

export interface TabData {
    tabTreatmentStrategies: TreatmentStrategy[];
    tabSelectedTreatmentStrategy: TreatmentStrategy;
    tabSelectedTreatment: Treatment;
}

export const emptyTabData: TabData = {
    tabTreatmentStrategies: [],
    tabSelectedTreatmentStrategy: {...emptyTreatmentStrategy},
    tabSelectedTreatment: {...emptyTreatment}
};