import {isNil, isEmpty} from 'ramda';

/**
 * Whether or not the specified item has a value (is not null, is not undefined, and is not considered empty)
 * @param item The item to check
 */
export const hasValue = (item: any) => {
    return !isNil(item) && !isEmpty(item);
};
