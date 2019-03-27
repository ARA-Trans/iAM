import {Scenario} from '@/shared/models/iAM/scenario';
import {InventoryItem, InventoryItemDetail} from '@/shared/models/iAM/inventory';
import {InvestmentStrategy, InvestmentStrategyBudgetYear} from '@/shared/models/iAM/investment';
import moment from 'moment';
import {emptyPerformanceStrategy, PerformanceStrategy} from '@/shared/models/iAM/performance';

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

export const mockInventory: InventoryItem[] = [{
    networkId: 1111,
    simulationId: 2222,
    referenceId: 1234567890,
    referenceKey: 19876,
}];

export const mockInventoryItemDetail: InventoryItemDetail = {
    simulationId: 2222,
    label: '5A02',
    name: 'Ben Franklin Bridge',
    location: [
        {label: '5A04 District', value: ''},
        {label: '5A05 County', value: ''},
        {label: '5A06 City/Town Place', value: ''},
        {label: '5A07 Feature Intersected', value: ''},
        {label: '5A08 Facility Carried', value: ''},
        {label: '5A09 Location', value: ''}
    ],
    ageAndService: [
        {label: '5A15 Year Built', value: ''},
        {label: '5A16 Year Reconstruction', value: ''},
        {label: '5A17 Type of Service On', value: ''},
        {label: '5A18 Type of Service Under', value: ''}
    ],
    management: [
        {label: '5A20 Maint Resp', value: ''},
        {label: '5A21 Owner', value: ''},
        {label: '5A23 Agency Admin Area', value: ''},
        {label: '5A24 Reporting Group', value: ''},
        {label: '6A06 Submitting Agency Classification', value: ''},
        {label: 'SE01 NBIS Bridge Length', value: ''},
        {label: 'SE04 Hist Significance', value: ''},
        {label: 'SE05 SHP Key Number', value: ''},
        {label: '6A19 Business Plan Network', value: ''}
    ],
    deckInformation: [
        {label: '5B01 Deck Structure Type', value: ''},
        {label: '6A38 Deck Sheet Type (PennDOT)', value: ''},
        {label: '5B02 Deck Surface Type', value: ''},
        {label: '5B03 Deck Membrane Type', value: ''},
        {label: '5B04 Deck Protection', value: ''},
        {label: '5B07 Deck Width (0/0)', value: ''},
        {label: '5B09 Skew', value: ''}
    ],
    spanInformation: [
        {label: '5B11 Number of Main Spans', value: ''},
        {label: '5B12 Main Span Material', value: ''},
        {label: '5B13 Main Span Design', value: ''},
        {label: '5B14 Number of Approach Spans', value: ''},
        {label: '5B15 Approach Span Material', value: ''},
        {label: '5B16 Approach Span Design', value: ''},
        {label: '5B17 Maximum Span Length', value: ''},
        {label: '5B18 Structure Length', value: ''},
        {label: '5B19 Deck Area', value: ''},
        {label: '5B20 Total Length', value: ''},
        {label: '6A44 FC Group Number (Main)', value: ''},
        {label: '6A44 FC Group Number (Approach)', value: ''}

    ],
    currentConditionDuration: [
        {name: 'DECK', condition: '', duration: 0},
        {name: 'SUPER', condition: '', duration: 0},
        {name: 'SUB', condition: '', duration: 0},
        {name: 'CULV', condition: '', duration: 0}
    ],
    previousConditionDuration: [
        {name: 'DECK', condition: '', duration: 0},
        {name: 'SUPER', condition: '', duration: 0},
        {name: 'SUB', condition: '', duration: 0},
        {name: 'CULV', condition: '', duration: 0}
    ],
    riskScores: {
        new: 0,
        old: 0
    },
    operatingRatingInventoryRatingGrouping: {
        ratingRows: [
            {
                operatingRating: {label: '4B05 NBI OR', value: ''},
                inventoryRating: {label: '4B07 NBI IR', value: ''},
                ratioLegalLoad: {label: 'Ratio OR / Max Legal Load', value: ''}
            },
            {
                operatingRating: {label: '4B09 H20 (OR)', value: ''},
                inventoryRating: {label: '4B11 H20 (IR)', value: ''},
                ratioLegalLoad: {label: 'Ratio OR / Max Legal Load', value: ''}
            },
            {
                operatingRating: {label: '4B12 ML80 (OR)', value: ''},
                inventoryRating: {label: '4B12 ML80 (IR)', value: ''},
                ratioLegalLoad: {label: 'Ratio OR / Max Legal Load', value: ''}
            },
            {
                operatingRating: {label: '4B13 TK527 (OR)', value: ''},
                inventoryRating: {label: '4B13 TK527 (IR)', value: ''},
                ratioLegalLoad: {label: 'Ratio OR / Max Legal Load', value: ''}
            }
        ],
        minRatioLegalLoad: {label: 'Min Ratio OR / Max Legal Load', value: ''}
    },
    nbiLoadRating: [
        {label: 'IR04 Load Type', value: ''},
        {label: 'IR05 NBI', value: ''},
        {label: 'IR010 Inv Rating Ton', value: ''},
        {label: 'IR11 Opr Rating Ton', value: ''},
        {label: 'IR11a SLC Rating Factor', value: ''},
        {label: 'IR20 IR Rating Factor', value: ''},
        {label: 'IR21 OR Rating Factor', value: ''},
        {label: 'IR17 Rating Dataset', value: ''}
    ],
    posting: [
        {label: 'VP01 Status Date', value: ''},
        {label: 'VP02 Posting Status', value: ''},
        {label: 'VP03 Special Restrictive Posting', value: ''},
        {label: 'VP04 Posted Weight Limit', value: ''},
    ],
    roadwayInfo: [
        {label: '5C10 ADT', value: ''},
        {label: '5C22 Func. Class', value: ''},
        {label: '4A15 Over Street Clearance', value: ''},
        {label: '4A17 Under Clearance', value: ''},
        {label: '5C29 NHS', value: ''}
    ]
};
/*******************************************INVESTMENT EDITOR MOCK DATA************************************************/
export const mockBudgetOrder: string[] = [
    'Budget A',
    'Budget B',
    'Budget C',
    'Budget D',
    'Budget E',
    'Budget F',
    'Budget G',
    'Budget H',
    'Budget I',
    'Budget J',
    'Budget K',
    'Budget L'
];

function createBudgetYearMockData() {
    const budgetYears: InvestmentStrategyBudgetYear[] = [];
    let budgetAAmount = 100000000;
    let budgetBAmount = 200000000;
    let budgetCAmount = 300000000;
    const currentDate = moment();
    const startYear = parseInt(currentDate.clone().format('YYYY'));
    const endYear = parseInt(currentDate.clone().add(20, 'years').format('YYYY'));
    for (let i = startYear; i <= endYear; i++) {
        budgetYears.push({
            year: i,
            budgets:  mockBudgetOrder.map((budget: string) => {
                switch (budget) {
                    case 'Budget A':
                        return {
                            name: budget,
                            amount: budgetAAmount
                        };
                    case 'Budget B':
                        return {
                            name: budget,
                            amount: budgetBAmount
                        };
                    case 'Budget C':
                        return {
                            name: budget,
                            amount: budgetCAmount
                        };
                    default:
                        return {
                            name: budget,
                            amount: 0
                        };
                }
            })
        });
        budgetAAmount += 500000;
        budgetBAmount += 500000;
        budgetCAmount += 500000;
    }
    return budgetYears;
}

export const mockInvestmentBudgetYears: InvestmentStrategyBudgetYear[] = createBudgetYearMockData();


export const mockInvestmentStrategies: InvestmentStrategy[] = [
    {
        networkId: 9876543210,
        simulationId: 1234567890,
        name: 'Investment Strategy 1',
        inflationRate: 3,
        discountRate: 2,
        budgetYears: mockInvestmentBudgetYears,
        budgetOrder: mockBudgetOrder,
        description: ''
    },
    {
        networkId: 9876543210,
        simulationId: 1234567891,
        name: 'Investment Strategy 2',
        inflationRate: 2,
        discountRate: 3,
        budgetYears: mockInvestmentBudgetYears,
        budgetOrder: mockBudgetOrder,
        description: ''
    }
];
/*******************************************PERFORMANCE EDITOR MOCK DATA***********************************************/
export const mockPerformanceStrategies: PerformanceStrategy[] = [
    {
        id: 1,
        name: 'Performance Strategy 1',
        description: 'This is a mock performance strategy',
        performanceStrategyEquations: [
            {
                performanceStrategyId: 1,
                performanceStrategyEquationId: 1,
                attribute: 'PCI',
                equationName: 'PCI Deduct Curve',
                equation: '(0,9)(1,8.68)(2,8.36)(3,8.04)(4,7.72)(5,7.58)(6,7.44)(7,7.3)(8,7.16)(9,7.02)(10,6.88)(11,6.78)(12,6.68)(13,6.58)(14,6.48)(15,6.38)(16,6.28)(17,6.18)(18,6.08)(19,5.98)(20,5.89)(21,5.8)(22,5.71)(23,5.62)(24,5.53)(25,5.44)(26,5.35)(27,5.26)(28,5.17)(29,5.08)(30,4.99)(31,4.92)(32,4.85)(33,4.78)(34,4.71)(35,4.64)(36,4.57)(37,4.5)(38,4.43)(39,4.36)(40,4.29)(41,4.22)(42,4.15)(43,4.08)(44,4.01)(45,3.94)(46,3.87)(47,3.8)(48,3.73)(49,3.66)(50,3.59)(51,3.52)(52,3.45)(53,3.38)(54,3.31)(55,3.24)(56,3.17)(57,3.1)(58,3.03)(59,2.96)(60,2.82)(61,2.68)(62,2.54)(63,2.4)(64,2.26)(65,2.12)(66,1.98)(67,1.97)(68,1.96)(69,1.95)(70,1.94)(71,1.93)(72,1.92)(73,1.91)(74,1.9)(75,1.89)(76,1.88)(77,1.87)(78,1.86)(79,1.85)(80,1.84)(81,1.83)(82,1.82)(83,1.81)(84,1.8)(85,1.79)(86,1.78)(87,1.77)(88,1.76)(89,1.75)(90,1.74)(91,1.73)(92,1.72)(93,1.71)(94,1.7)(95,1.69)(96,1.68)(97,1.67)(98,1.66)(99,1.65)(100,1.64)(101,1.63)(102,1.62)(103,1.61)(104,1.6)(105,1.59)(106,1.58)(107,1.57)(108,1.56)(109,1.55)(110,1.54)(111,1.53)(112,1.52)(113,1.51)(114,1.5)(115,1.49)(116,1.48)(117,1.47)(118,1.46)(119,1.45)(120,1.44)(121,1.43)(122,1.42)(123,1.41)(124,1.4)(125,1.39)(126,1.38)(127,1.37)(128,1.36)(129,1.35)(130,1.34)(131,1.33)(132,1.32)(133,1.31)(134,1.3)(135,1.29)(136,1.28)(137,1.27)(138,1.26)(139,1.25)(140,1.24)(141,1.23)(142,1.22)(143,1.21)(144,1.2)(145,1.19)(146,1.18)(147,1.17)(148,1.16)(149,1.15)(150,1.14)(151,1.13)(152,1.12)(153,1.11)(154,1.1)(155,1.09)(156,1.08)(157,1.07)(158,1.06)(159,1.05)(160,1.04)(161,1.03)(162,1.02)(163,1.01)(164,1)(165,0.99)(166,0.98)(167,0.97)(168,0.96)(169,0.95)(170,0.94)(171,0.93)(172,0.92)(173,0.91)(174,0.9)(175,0.89)(176,0.88)(177,0.87)(178,0.86)(179,0.85)(180,0.84)(181,0.83)(182,0.82)(183,0.81)(184,0.8)(185,0.79)(186,0.78)(187,0.77)(188,0.76)(189,0.75)(190,0.74)(191,0.73)(192,0.72)(193,0.71)(194,0.7)(195,0.69)(196,0.68)(197,0.67)(198,0.66)(199,0.65)(200,0.64)(201,0.63)(202,0.62)(203,0.61)(204,0.6)(205,0.59)(206,0.58)(207,0.57)(208,0.56)(209,0.55)(210,0.54)(211,0.53)(212,0.52)(213,0.51)(214,0.5)(215,0.49)(216,0.48)(217,0.47)(218,0.46)(219,0.45)(220,0.44)(221,0.43)(222,0.42)(223,0.41)(224,0.4)(225,0.39)(226,0.38)(227,0.37)(228,0.36)(229,0.35)(230,0.34)(231,0.33)(232,0.32)(233,0.31)(234,0.3)(235,0.29)(236,0.28)(237,0.27)(238,0.26)(239,0.25)(240,0.24)(241,0.23)(242,0.22)(243,0.21)(244,0.2)(245,0.19)(246,0.18)(247,0.17)(248,0.16)(249,0.15)(250,0.14)(251,0.13)(252,0.12)(253,0.11)(254,0.1)(255,0.09)(256,0.08)(257,0.07)(258,0.06)(259,0.05)(260,0.04)(261,0.03)(262,0.02)(263,0.01)',
                criteria: '[ADTTOTAL]=\'50\' OR [ADTYEAR]=\'1999\' AND [AGE]>\'30\' OR [APPRALIGN]>=\'7\' OR [APPRALIGN]<=\'5\'',
                shift: false,
                piecewise: false,
                isFunction: false

            }
        ]
    }
];
