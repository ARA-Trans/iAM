import * as R from 'ramda';

/**
 * Whether or not the specified item has a value (is not null, is not undefined, and is not considered empty)
 * @param item The item to check
 */
export const hasValue = (item: any) => {
    return !R.isNil(item) && !R.isEmpty(item);
};