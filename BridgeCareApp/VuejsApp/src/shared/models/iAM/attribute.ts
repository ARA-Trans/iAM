export interface Attribute {
    name: string;
    type: string;
}

export interface NetworkAttribute {
    networkId: number;
    attribute: string;
}

export interface AttributeSelectValues {
    attribute: string;
    values: string[];
}

export interface AttributeSelectValuesResult {
    values: string[];
    resultMessage: string;
}
