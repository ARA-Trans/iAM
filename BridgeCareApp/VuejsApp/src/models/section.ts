export interface IAttributeYearlyValue {
    year: number;
    value: any;
}

export interface IAttribute {
    name: string;
    yearlyValues: IAttributeYearlyValue[];
}

export interface ILocation {
    lat: number;
    long: number;
}

export interface ISection {
    sectionId: number;
    location: ILocation;
    attributes: IAttribute[];
}
/***********************************************MOCK DATA**************************************************************/
export const mockLengthValues: IAttributeYearlyValue[] = [
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
export const mockLengthAttribute: IAttribute = {
    name: 'LENGTH',
    yearlyValues: mockLengthValues
};

export const mockAttrDeckAreaValues: IAttributeYearlyValue[] = [
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
export const mockDeckAreaAttribute: IAttribute = {
    name: 'DECK_AREA',
    yearlyValues: mockAttrDeckAreaValues
};

export const mockAdtTotalValues: IAttributeYearlyValue[] = [
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
export const mockAdtTotalAttribute: IAttribute = {
    name: 'ADTTOTAL',
    yearlyValues: mockAdtTotalValues
};

export const mockBusPlanNetworkValues: IAttributeYearlyValue[] = [
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
export const mockBusPlanNetworkAttribute: IAttribute = {
    name: 'BUS_PLAN_NETWORK',
    yearlyValues: mockBusPlanNetworkValues
};

export const mockBridgeTypeValues: IAttributeYearlyValue[] = [
    {
        year: 2019,
        value: 'B',
    },
    {
        year: 2018,
        value: 'B'
    }
];
export const mockBridgeTypeAttribute: IAttribute = {
    name: 'BRIDGE_TYPE',
    yearlyValues: mockBridgeTypeValues
};

export const mockConditionIndexValues: IAttributeYearlyValue[] = [
    {
        year: 2019,
        value: 10,
    }
];
export const mockConditionIndexAttribute: IAttribute = {
    name: 'CONDITIONINDEX',
    yearlyValues: mockConditionIndexValues
};

export const mockFamilyIdValues: IAttributeYearlyValue[] = [
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
export const mockFamilyIdAttribute: IAttribute = {
    name: 'FAMILY_ID',
    yearlyValues: mockFamilyIdValues
};

export const mockSections: ISection[] = [
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
