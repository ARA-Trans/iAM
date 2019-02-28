export interface AttributeYearlyValue {
    year: number;
    value: any;
}

export interface Attribute {
    name: string;
    yearlyValues: AttributeYearlyValue[];
}

export interface LatLong {
    lat: number;
    long: number;
}

export interface SectionDetail {
    sectionId: number;
    location: LatLong;
    attributes: Attribute[];
}

export interface Section {
    referenceId: number;
    referenceKey: number;
    sectionId: number;
    networkId: number;
}
/***********************************************MOCK DATA**************************************************************/
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

export const mockSectionDetail: SectionDetail = {
    sectionId: 1000004,
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
};

export const mockSections: Section[] = [
    {
        referenceId: 15003004520000,
        referenceKey: 10001,
        sectionId: 1000004,
        networkId: 13
    }
];
