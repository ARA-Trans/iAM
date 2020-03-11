import {isNil, isEmpty} from 'ramda';

/**
 * Whether or not the specified item has a value (is not null, is not undefined, and is not considered empty)
 * @param item The item to check
 * @param itemProp The item's property to check for a value
 */
export const hasValue = (item: any, itemProp: string = '') => {
    const itemHasValue = item !== null && item !== undefined && item !== '' && item !== [];

    let itemPropHasValue = true;
    if (itemHasValue && itemProp !== '') {
        itemPropHasValue = item.hasOwnProperty(itemProp) && item[itemProp] !== null && item[itemProp] !== undefined &&
            item[itemProp] !== '' && item[itemProp] !== [];
    }

    return itemHasValue && itemPropHasValue;
};
