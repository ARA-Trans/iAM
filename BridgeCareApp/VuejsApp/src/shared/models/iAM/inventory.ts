export interface InventoryItem {
    networkId: number;
    simulationId: number;
    referenceId: number;
    referenceKey: number;
}

export interface LabelValue {
    label: string;
    value: any;
}

export interface ConditionDuration {
    name: string;
    condition: string;
    duration: number;
}

export interface RiskScores {
    new: number;
    old: number;
}

export interface OperatingRatingInventoryRatingRow {
    operatingRating: LabelValue;
    inventoryRating: LabelValue;
    ratioLegalLoad: LabelValue;
}

export interface OperatingRatingInventoryRatingGrouping {
    ratingRows: OperatingRatingInventoryRatingRow[];
    minRatioLegalLoad: LabelValue;
}

export interface InventoryItemDetail {
    simulationId: number;
    label: string;
    name: string;
    location: LabelValue[];
    ageAndService: LabelValue[];
    management: LabelValue[];
    deckInformation: LabelValue[];
    spanInformation: LabelValue[];
    currentConditionDuration: ConditionDuration[];
    previousConditionDuration: ConditionDuration[];
    riskScores: RiskScores;
    operatingRatingInventoryRatingGrouping: OperatingRatingInventoryRatingGrouping;
    nbiLoadRating: LabelValue[];
    posting: LabelValue[];
    roadwayInfo: LabelValue[];
}

export const emptyRiskScores: RiskScores = {
    new: 0,
    old: 0
};

export const emptyInventoryItemDetail: InventoryItemDetail = {
    simulationId: 0,
    label: '',
    name: '',
    location: [],
    ageAndService: [],
    management: [],
    deckInformation: [],
    spanInformation: [],
    currentConditionDuration: [],
    previousConditionDuration: [],
    riskScores: emptyRiskScores,
    operatingRatingInventoryRatingGrouping: {
        ratingRows: [],
        minRatioLegalLoad: {
            label: '',
            value: ''
        }
    },
    nbiLoadRating: [],
    posting: [],
    roadwayInfo: []
};