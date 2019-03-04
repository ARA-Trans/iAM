export interface AttributesWithYearlyValues {
    year: number;
    value: any;
}

export interface Attribute {
    name: string;
    yearlyValues: AttributesWithYearlyValues[];
}

export interface LatitudeLongitude {
    lat: number;
    long: number;
}

export interface Section {
    sectionId: number;
    location: LatitudeLongitude;
    attributes: Attribute[];
}
