import {PerformanceLibrary} from '@/shared/models/iAM/performance';
import {
    Cost,
    emptyFeasibility,
    Feasibility,
    Treatment,
    TreatmentLibrary
} from '@/shared/models/iAM/treatment';
/*******************************************PERFORMANCE EDITOR MOCK DATA***********************************************/
export const mockPerformanceLibraries: PerformanceLibrary[] = [
    {
        id: 1,
        name: 'Mock Performance Library',
        description: 'This is a mock performance library',
        equations: []
    }
];
/*******************************************TREATMENT EDITOR MOCK DATA*************************************************/
export const mockFeasibility: Feasibility = {
    treatmentId: 1,
    id: 1,
    criteria: '[ROADWIDTH]>=\'60\' AND [ROADWIDTH]<\'75\' AND [TRUCKPCT]=\'Low\' AND [UNDERCLR]<\'75\' AND [SUFF_RATE]<\'2\'',
    yearsBeforeAny: 5,
    yearsBeforeSame: 7
};

export const mockCost: Cost = {
    treatmentId: 1,
    id: 1,
    equation: '[DECK_AREA]*1.99',
    isFunction: false,
    criteria: '[DISTRICT]=\'1\''
};

export const mockTreatments: Treatment[] = [
    {
        treatmentLibraryId: 1,
        id: 1,
        name: 'Chip Seal',
        feasibility: mockFeasibility,
        costs: [mockCost],
        consequences: [],
        budgets: []
    },
    {
        treatmentLibraryId: 1,
        id: 2,
        name: '2\'\' Mill 2\'\' Fill',
        feasibility: emptyFeasibility,
        costs: [],
        consequences: [],
        budgets: []
    }
];

export const mockTreatmentLibraries: TreatmentLibrary[] = [
    {
        id: 1,
        name: 'Mock Treatment Library',
        description: 'This is a mock treatment library',
        treatments: mockTreatments
    }
];
