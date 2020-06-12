export interface Attribute {
    name: string;
    type: string;
}

export interface NetworkAttributes {
    networkId: number;
    attributes: string[];
}

export interface AttributeSelectValues {
    attribute: string;
    values: string[];
}

export interface AttributeSelectValuesResult {
    attribute: string;
    values: string[];
    resultMessage: string;
}
