/**
 * Whether or not the specified item has a value (is not null, is not undefined, and is not considered empty)
 * @param item The item to check
 * @param itemProp The item's property to check for a value
 */
export const hasValue = (item: any, itemProp: string = '') => {
    let itemHasValue = item !== null && item !== undefined && item !== '';
    if (Array.isArray(item)) 
    {    itemHasValue = itemHasValue && item.length > 0;}

    let itemPropHasValue = true;
    if (itemHasValue && itemProp !== '') {
        itemPropHasValue = item.hasOwnProperty(itemProp) && item[itemProp] !== null && item[itemProp] !== undefined &&
            item[itemProp] !== '' && item[itemProp] !== [];
    }

    return itemHasValue && itemPropHasValue;
};
