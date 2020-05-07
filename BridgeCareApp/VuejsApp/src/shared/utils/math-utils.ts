export const bytesToMegabytes = (bytes: number) => {
    return (bytes / Math.pow(1024, 2)).toFixed(2);
};
