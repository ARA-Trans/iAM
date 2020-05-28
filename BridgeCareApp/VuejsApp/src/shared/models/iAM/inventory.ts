export interface InventoryItem {
    bmsId: string;
    brKey: string;
}

export interface LabelValue {
    label: string;
    value: any;
}

export interface ConditionDuration {
    name: string;
    condition: string;
    duration: string;
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

export interface NbiLoadRating {
    nbiLoadRatingRow: LabelValue[];
}

export interface InventoryItemDetail {
    bmsId: string;
    brKey: string;
    name: string;
    label: string;
    location: LabelValue[];
    ageAndService: LabelValue[];
    management: LabelValue[];
    deckInformation: LabelValue[];
    spanInformation: LabelValue[];
    currentConditionDuration: ConditionDuration[];
    previousConditionDuration: ConditionDuration[];
    riskScores: RiskScores;
    operatingRatingInventoryRatingGrouping: OperatingRatingInventoryRatingGrouping;
    nbiLoadRatings: NbiLoadRating[];
    posting: LabelValue[];
    roadwayInfo: LabelValue[];
}

export const emptyRiskScores: RiskScores = {
    new: 0,
    old: 0
};

export const emptyInventoryItemDetail: InventoryItemDetail = {
    bmsId: 0,
    brKey: 0,
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
    nbiLoadRatings: [{nbiLoadRatingRow: []}],
    posting: [],
    roadwayInfo: []
};
