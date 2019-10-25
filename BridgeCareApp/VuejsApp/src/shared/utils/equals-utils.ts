import {hasValue} from '@/shared/utils/has-value-util';
import {equals} from 'ramda';
import {sortByProperty} from '@/shared/utils/sorter-utils';
import {getPropertyValues} from '@/shared/utils/getter-utils';

export const itemsEqual = (item1: any | any[], item2: any | any[], property?: string, comparePropertyValues: boolean = false) => {
    if (hasValue(property)) {
        if (comparePropertyValues) {
            item1 = getPropertyValues(property as string, sortByProperty(property as string, item1));
            item2 = getPropertyValues(property as string, sortByProperty(property as string, item2));
        } else {
            item1 = sortByProperty(property as string, item1);
            item2 = sortByProperty(property as string, item2);
        }
    }

    return equals(item1, item2);
};
