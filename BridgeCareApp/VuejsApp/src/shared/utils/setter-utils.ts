import {lensIndex, lensProp, set} from 'ramda';

export const setItemPropertyValueInList = (index: number, property: string, value: any, items: any[]) => {
    return set(lensIndex(index), set(lensProp(property), value, items[index]), items);
};
