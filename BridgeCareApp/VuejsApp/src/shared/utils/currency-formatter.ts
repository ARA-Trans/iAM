export const formatAsCurrency = (value: any) => {
    if (isNaN(value)) {
        return value;
    }

    return new Intl
        .NumberFormat('en-US', {style: 'currency', currency: 'USD'})
        .format(value);
};
