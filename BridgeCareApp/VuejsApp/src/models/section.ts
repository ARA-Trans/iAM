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

export interface Section {
    sectionId: number;
    location: LatLong;
    attributes: Attribute[];
}
