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
