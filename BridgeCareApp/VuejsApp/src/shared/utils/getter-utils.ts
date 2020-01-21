import {last, pluck, uniq} from 'ramda';
import {sorter} from '@/shared/utils/sorter-utils';

export const getPropertyValuesNonUniq = (property: string, items: any[]): any[] => {
    const getter = pluck(property);
    return getter(items);
};

export const getPropertyValues = (property: string, items: any[]): any[] => {
    const getter = pluck(property);
    return uniq(getter(items));
};

export const getLatestPropertyValue = (property: string, items: any[]): any => {
    return last(sorter(getPropertyValues(property, items)) as any[]);
};