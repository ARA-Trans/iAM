import {lensIndex, lensProp, set} from 'ramda';

export const setItemPropertyValueInList = (index: number, property: string, value: any, items: any[]) => {
    return set(lensIndex(index), set(lensProp(property), value, items[index]), items);
};

export const setItemPropertyValue = (property: string, value: any, item: any) => {
    return set(lensProp(property), value, item);
};
