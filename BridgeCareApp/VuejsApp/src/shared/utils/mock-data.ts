import {Scenario} from '@/models/scenario';
import {Attribute, AttributeYearlyValue, Section} from '@/models/section';
import {InvestmentStrategy, InvestmentStrategyDetail} from '@/models/investment';

/******************************************CRITERIA EDITOR MOCK DATA***************************************************/
export const attributes: string[] = [
    'ADTTOTAL',
    'ADTYEAR',
    'AGE',
    'APPRALIGN',
    'AROADWIDTH',
    'BRIDGE_TYPE',
    'BRIDGE_TYPE_ALT',
    'BUS_PLAN_NETWORK',
    'COMB',
    'CONDITIONINDEX',
    'COUNTY',
    'CULV',
    'CULV_SEEDED',
    'DECK',
    'DECK_AREA',
    'DECK_SEEDED',
    'DECK_WIDTH',
    'DECKGEEOM',
    'DEFHWY',
    'DISTRICT',
    'FAMILY_ID',
    'FEATURE_CARRIED',
    'FEATURE_INTERSECTED',
    'FUNC_CLASS',
    'HBRR_ELIG',
    'INSPTYPE',
    'IRLOAD',
    'LANE',
    'LAT',
    'LENGTH',
    'LOCATION',
    'LONG',
    'MPO',
    'MSG',
    'MUNI_CODE',
    'NBI_RATING',
    'NBISLEN',
    'NHS_IND',
    'NUMBER_SPANS',
    'OVER_WATER',
    'OWNER_CODE',
    'POST_LIMIT_COMB',
    'POST_LIMIT_WEIGHT',
    'POST_STATUS',
    'POST_STATUS_DATE',
    'PROPWORK',
    'ROADWIDTH',
    'SD_RISK_SCORE',
    'SINGLE',
    'SPEC_RESTRICT_POST',
    'STRRATING',
    'STRUCTURE_TYPE',
    'SUB',
    'SUB_SEEDED',
    'SUBM_AGENCY',
    'SUFF_RATE',
    'SUMLANE',
    'SUP',
    'SUP_SEEDED',
    'TRUCKPCT',
    'UNDERCLR',
    'WATERADEQ',
    'YEAR_BUILT',
    'YEAR_RECON'
];

/********************************************SCENARIOS MOCK DATA*******************************************************/
export const userScenarios: Scenario[] = [
    {
        scenarioId: 1,
        name: 'Scenario A-1',
        createdDate: new Date(),
        lastModifiedDate: new Date(),
        status: true,
        shared: false
    },
    {
        scenarioId: 2,
        name: 'Scenario A-2',
        createdDate: new Date(),
        lastModifiedDate: new Date(),
        status: true,
        shared: false
    },
    {
        scenarioId: 3,
        name: 'Scenario A-3',
        createdDate: new Date(),
        lastModifiedDate: new Date(),
        status: false,
        shared: false
    },
    {
        scenarioId: 4,
        name: 'Scenario B-1',
        createdDate: new Date(),
        lastModifiedDate: new Date(),
        status: true,
        shared: false
    },
    {
        scenarioId: 5,
        name: 'Scenario B-2',
        createdDate: new Date(),
        lastModifiedDate: new Date(),
        status: true,
        shared: false
    },
    {
        scenarioId: 6,
        name: 'Scenario B-3',
        createdDate: new Date(),
        lastModifiedDate: new Date(),
        status: true,
        shared: false
    }
];

export const sharedScenarios: Scenario[] = [
    {
        scenarioId: 7,
        name: 'Scenario C-1',
        createdDate: new Date(),
        lastModifiedDate: new Date(),
        status: true,
        shared: true
    },
    {
        scenarioId: 8,
        name: 'Scenario C-2',
        createdDate: new Date(),
        lastModifiedDate: new Date(),
        status: true,
        shared: true
    },
    {
        scenarioId: 9,
        name: 'Scenario C-3',
        createdDate: new Date(),
        lastModifiedDate: new Date(),
        status: false,
        shared: true
    }
];
/*******************************************INVENTORY MOCK DATA********************************************************/
export const mockLengthValues: AttributeYearlyValue[] = [
    {
        year: 2019,
        value: 100,
    },
    {
        year: 2018,
        value: 100,
    },
    {
        year: 2017,
        value: 100,
    }
];
export const mockLengthAttribute: Attribute = {
    name: 'LENGTH',
    yearlyValues: mockLengthValues
};

export const mockAttrDeckAreaValues: AttributeYearlyValue[] = [
    {
        year: 2019,
        value: 4781.25,
    },
    {
        year: 2018,
        value: 4781.25,
    },
    {
        year: 2017,
        value: 4781.25,
    }
];
export const mockDeckAreaAttribute: Attribute = {
    name: 'DECK_AREA',
    yearlyValues: mockAttrDeckAreaValues
};

export const mockAdtTotalValues: AttributeYearlyValue[] = [
    {
        year: 2019,
        value: 1065,
    },
    {
        year: 2018,
        value: 1060
    },
    {
        year: 2017,
        value: 1055
    }
];
export const mockAdtTotalAttribute: Attribute = {
    name: 'ADTTOTAL',
    yearlyValues: mockAdtTotalValues
};

export const mockBusPlanNetworkValues: AttributeYearlyValue[] = [
    {
        year: 2019,
        value: 2,
    },
    {
        year: 2018,
        value: 2
    },
    {
        year: 2017,
        value: 1
    }
];
export const mockBusPlanNetworkAttribute: Attribute = {
    name: 'BUS_PLAN_NETWORK',
    yearlyValues: mockBusPlanNetworkValues
};

export const mockBridgeTypeValues: AttributeYearlyValue[] = [
    {
        year: 2019,
        value: 'B',
    },
    {
        year: 2018,
        value: 'B'
    }
];
export const mockBridgeTypeAttribute: Attribute = {
    name: 'BRIDGE_TYPE',
    yearlyValues: mockBridgeTypeValues
};

export const mockConditionIndexValues: AttributeYearlyValue[] = [
    {
        year: 2019,
        value: 10,
    }
];
export const mockConditionIndexAttribute: Attribute = {
    name: 'CONDITIONINDEX',
    yearlyValues: mockConditionIndexValues
};

export const mockFamilyIdValues: AttributeYearlyValue[] = [
    {
        year: 2019,
        value: 3,
    },
    {
        year: 2018,
        value: 3
    },
    {
        year: 2017,
        value: 3
    }
];
export const mockFamilyIdAttribute: Attribute = {
    name: 'FAMILY_ID',
    yearlyValues: mockFamilyIdValues
};

export const mockSections: Section[] = [
    {
        sectionId: 60006206001408,
        location: {lat: 39.949, long: -75.139},
        attributes: [
            mockLengthAttribute,
            mockDeckAreaAttribute,
            mockAdtTotalAttribute,
            mockBusPlanNetworkAttribute,
            mockBridgeTypeAttribute,
            mockConditionIndexAttribute,
            mockFamilyIdAttribute
        ]
    }
];
/*******************************************INVESTMENT EDITOR MOCK DATA************************************************/
export const mockInvestmentStrategies: InvestmentStrategy[] = [
    {
        id: 1234567890,
        name: 'Investment Strategy 1'
    },
    {
        id: 1234567891,
        name: 'Investment Strategy 2'
    }
];

export const mockInvestmentStrategy1Detail: InvestmentStrategyDetail = {
    investmentStrategyId: 1234567890,
    inflationRate: 3.15,
    discountRate: 1.345,
    year: 2019,
    budgets: [],
    description: ''
};