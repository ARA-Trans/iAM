import {Scenario} from '@/shared/models/iAM/scenario';
import {InventoryItem, InventoryItemDetail} from '@/shared/models/iAM/inventory';
import {InvestmentStrategy, InvestmentStrategyBudgetYear} from '@/shared/models/iAM/investment';
import moment from 'moment';

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